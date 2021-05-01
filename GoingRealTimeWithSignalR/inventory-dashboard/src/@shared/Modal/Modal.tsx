import classNames from 'classnames';
import React, { useEffect } from 'react';
import { fromEvent, Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { useStyles } from './Modal.style';

export interface ModalProps {
  onClose: Subject<any>;
  isClosableByEscape: boolean;
  isClosableByOverlay: boolean;
  isPinnedToTop?: boolean;
}

export const Modal: React.FC<ModalProps> = props => {
  const s = useStyles();
  const destroy$ = new Subject();

  useEffect(() => {
    if (props.isClosableByEscape) {
      subscribeOnEscape();
    }
  },        []);

  const subscribeOnEscape = () => {
    fromEvent<KeyboardEvent>(document, 'keyup')
      .pipe(
        takeUntil(destroy$),
        filter((e: KeyboardEvent) => e.key === 'Escape'),
      )
      .subscribe(() => props.onClose.next());
  };

  useEffect(() => {
    return () => {
      destroy$.next();
    };
  },        []);

  const onOverlayClick = () => {
    const { isClosableByOverlay, onClose } = props;
    if (isClosableByOverlay) {
      onClose.next();
    }
  };

  return (
    <div className={classNames(s.container, { [s.pinnedToTop]: props.isPinnedToTop })}>
      <div className={s.overlay} onClick={onOverlayClick}/>
      <div className={s.instance}>
        <div className={s.content}>
          {props.children}
        </div>
      </div>
    </div>
  );
};
