import { IconButton } from '@material-ui/core';
import Close from '@material-ui/icons/Close';
import { forEach } from 'lodash';
import { autorun } from 'mobx';
import { inject, observer } from 'mobx-react';
import { withSnackbar, WithSnackbarProps } from 'notistack';
import React, { useEffect } from 'react';

import { APP_INJECTION_KEY, IInjectedAppStore } from '@shared/store/app/AppStore';
import { useStyles } from './SnackMessages.style';

export interface InjectedProps extends WithSnackbarProps, IInjectedAppStore {
}

export const SnackMessagesComponent: React.FC<WithSnackbarProps> = observer(props => {
  const app = (props as InjectedProps).app;

  const hideNotificationDelay = 250;
  const s = useStyles();

  const onOffline = () => {
    app.addNotification({ message: 'Connection failed. Please try again', options: { persist: true } });
  };

  const renderActionButton = (key: number) => {
    return (
      <IconButton
        color="inherit"
        className={s.actionComponent}
        onClick={() => closeNotification(key)}
      >
        <Close/>
      </IconButton>
    );
  };

  const closeNotification = (key: number) => {
    setTimeout(() => props.closeSnackbar(key), hideNotificationDelay);
  };

  useEffect(() => {
    window.addEventListener('offline', onOffline);
    autorun(
      () => {
        forEach(Array.from(app.notifications.values()), notification => {
          props.enqueueSnackbar(notification.message, {
            ...notification.options,
            action: renderActionButton,
          });
          app.removeNotification(notification);
        });
      },
    );
  },        []);

  return null;
});

export const SnackMessages = inject(APP_INJECTION_KEY)(withSnackbar(SnackMessagesComponent));
