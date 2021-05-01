import { Typography, Paper } from '@material-ui/core';
import React from 'react';

import { emptyScreenParams, EmptyScreenType } from './config';
import { useStyles } from './EmptyScreen.style';

interface Props {
  type: EmptyScreenType;
}

export const EmptyScreen: React.FC<Props> = ({ type }) => {
  const s = useStyles();

  const { header, text, imageUrl } = emptyScreenParams[type];
  const titleLevel = type === 'notFound' ? 'h5' : 'h4';

  return (
    <Paper className={s.emptyScreenWrapper}>
      <div className={s.emptyScreen}>
        <img src={imageUrl} className={s.image} />
        <Typography variant={titleLevel}>{header}</Typography>
        <Typography component="p" className={s.text}>{text}</Typography>
      </div>
    </Paper>
  );
};

EmptyScreen.displayName = 'EmptyScreen';
