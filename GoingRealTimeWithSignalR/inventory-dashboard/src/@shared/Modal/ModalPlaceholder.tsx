import { History, UnregisterCallback } from 'history';
import React, { useEffect, useState } from 'react';
import { fromEvent, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { modalService } from './modalService';

export interface ModalPlaceholderState {
  activeModal: any;
  history?: History;
}

export interface ModalPlaceholderProps {
  history?: History;
}

export const ModalPlaceholder: React.FC<ModalPlaceholderProps> = props => {
  const destroy$ = new Subject();
  let unsubscribeListener: UnregisterCallback;

  const [activeModal, setActiveModal] = useState(null);

  useEffect(() => {
    subscribeOnUrlChange();
    subscribeOnPageReload();
    subscribeOnActiveModal();
  },        []);

  const subscribeOnActiveModal = () => {
    modalService.activeModal$
      .pipe(
        takeUntil(destroy$),
      )
      .subscribe((activeModalElement: any) => {
        setActiveModal(activeModalElement);
      });
  };

  const subscribeOnPageReload = () => {
    fromEvent(window, 'beforeunload')
      .pipe(takeUntil(destroy$))
      .subscribe(() => {
        modalService.closeModal();
      });
  };

  const subscribeOnUrlChange = () => {
    const { history } = props;

    if (history) {
      unsubscribeListener = history.listen(() => {
        modalService.closeModal();
      });
    }
  };

  useEffect(() => {
    return () => {
      destroy$.next();

      if (unsubscribeListener) {
        unsubscribeListener();
      }
    };
  },        []);

  return activeModal;
};
