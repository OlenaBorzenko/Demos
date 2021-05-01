import React from 'react';

import { useStyles } from './Modal.style';

export const ModalWrapper: React.FC = ({ children }) => {
  const s = useStyles();

  return (
    <div className={s.wrapper}>
      {children}
    </div>
  );
};

ModalWrapper.displayName = 'ModalWrapper';
