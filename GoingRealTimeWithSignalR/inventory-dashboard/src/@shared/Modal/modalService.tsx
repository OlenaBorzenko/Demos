import autobind from 'autobind-decorator';
import React from 'react';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

import { Modal } from './Modal';

export type ModalInputComponentType<T = null> = {closeModal: (data?: T) => void};
type ContentComponent<InputProps, Output> = React.ComponentType<InputProps & ModalInputComponentType<Output>>;

export type ModalConfig = {
  isClosableByEscape: boolean;
  isClosableByOverlay: boolean;
  isPinnedToTop: boolean;
};

const defaultModalConfig: ModalConfig = {
  isClosableByEscape: true,
  isClosableByOverlay: false,
  isPinnedToTop: false,
};

class ModalService {
  static instance: ModalService;
  activeModal$ = new BehaviorSubject<null | React.ReactNode>(null);
  private onModalClose$ = new Subject<any>();
  private onCloseSubscription?: Subscription;
  private constructor () {}

  static getInstance (): ModalService {
    if (!this.instance) {
      this.instance = new ModalService();
    }

    return this.instance;
  }

  openModal<InputProps, Output = null> (
    component: ContentComponent<InputProps, Output>,
    props?: InputProps,
    modalConfig?: Partial<ModalConfig>)
    : Promise<null | Output> {
    const activeModalConfig: ModalConfig = Object.assign({}, defaultModalConfig, modalConfig);

    if (this.activeModal$.value) {
      this.closeModal();

      if (this.onCloseSubscription) {
        this.onCloseSubscription.unsubscribe();
      }
    }

    this.activeModal$.next(
      this.getWrappedElement<InputProps, Output>(component, props as InputProps, activeModalConfig),
    );

    return new Promise(resolve => {
      this.onCloseSubscription = this.onModalClose$
        .pipe(first())
        .subscribe((data?: Output) => {
          this.closeModal();
          resolve(data);
        });
    });
  }

  closeModal () {
    this.activeModal$.next(null);
  }

  @autobind
  private triggerModalClose<T> (data?: T) {
    this.onModalClose$.next(data);
  }

  private getWrappedElement<InputProps = undefined, Output = undefined> (
    Component: ContentComponent<InputProps, Output>,
    props: InputProps,
    modalConfig: ModalConfig,
  ) {
    return (
      <Modal
        onClose={this.onModalClose$}
        isClosableByEscape={modalConfig.isClosableByEscape}
        isClosableByOverlay={modalConfig.isClosableByOverlay}
        isPinnedToTop={modalConfig.isPinnedToTop}
      >
        <Component {...props} closeModal={this.triggerModalClose}/>
      </Modal>
    );
  }
}

export const modalService = ModalService.getInstance();
