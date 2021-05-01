import classNames from 'classnames';
import React from 'react';

import { useStyles } from './Modal.style';

interface Props {
  className?: string;
}

export const ModalFooter: React.FC<Props> = ({ children, className }) => {
  const s = useStyles();

  return (
    <main className={classNames(s.footer, className)}>
      {children}
    </main>
  );
};

ModalFooter.displayName = 'ModalFooter';
