import classNames from 'classnames';
import React from 'react';

import { useStyles } from './Modal.style';
import { Typography } from '@material-ui/core';

interface ModalBodyProps {
  className?: string;
  id?: string;
}

export const ModalBody: React.FC<ModalBodyProps> = ({ children, className, id }) => {
  const s = useStyles();

  return (
    <main className={classNames(s.body, className)} id={id}>
      <Typography variant="h5">
        {children}
      </Typography>
    </main>
  );
};

ModalBody.displayName = 'ModalBody';
