import { IconButton } from '@material-ui/core';
import Close from '@material-ui/icons/Close';
import { forEach } from 'lodash';
import { autorun } from 'mobx';
import { observer } from 'mobx-react';
import { withSnackbar, WithSnackbarProps } from 'notistack';
import React, { useEffect } from 'react';

import { IAppStore } from '@shared/store/app/AppStore';
import { useStyles } from './SnackMessages.style';
import { useStores } from '@shared/helpers';

export interface InjectedProps extends WithSnackbarProps {
}

export const SnackMessagesComponent: React.FC<WithSnackbarProps> = observer(props => {
  const app = useStores().app as IAppStore;

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

export const SnackMessages = withSnackbar(SnackMessagesComponent);
