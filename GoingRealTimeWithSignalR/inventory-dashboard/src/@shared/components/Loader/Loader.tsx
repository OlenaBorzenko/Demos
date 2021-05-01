import React from 'react';

import { CircularProgress } from '@material-ui/core';
import { useStyles } from './Loader.style';

interface Props {
}

export const Loader: React.FC<Props> = () => {
  const s = useStyles();

  return (
    <div>
      <CircularProgress className={s.progress} />
    </div>
  );
};

Loader.displayName = 'Loader';
