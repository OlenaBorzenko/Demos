import { Slide } from '@material-ui/core';
import { TransitionProps } from '@material-ui/core/transitions/transition';
import { SnackbarProviderProps } from 'notistack';
import React from 'react';

export const snackbarProviderConfig: Partial<SnackbarProviderProps> = {
  anchorOrigin: { horizontal: 'right', vertical: 'bottom' },
  autoHideDuration: 12 * 1000,
  hideIconVariant: true,
  maxSnack: 5,
  preventDuplicate: true,
  TransitionComponent: (props: TransitionProps) => <Slide {...props} direction="left" timeout={250}/>,
};
