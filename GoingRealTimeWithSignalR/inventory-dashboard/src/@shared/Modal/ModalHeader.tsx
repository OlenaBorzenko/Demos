import classNames from 'classnames';
import React from 'react';

import { useStyles } from './Modal.style';
import { Typography } from '@material-ui/core';

export interface ModalHeaderProps {
  className?: string;
}

export const ModalHeader: React.FC<ModalHeaderProps> = ({ children, className }) => {
  const s = useStyles();

  return (
    <header className={classNames(s.header, className)}>
      <Typography variant="h4">
        {children}
      </Typography>
    </header>
  );
};

ModalHeader.displayName = 'ModalHeader';
