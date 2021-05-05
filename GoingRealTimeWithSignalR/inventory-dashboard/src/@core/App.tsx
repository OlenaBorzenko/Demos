import { History } from 'history';
import { observer } from 'mobx-react';
import React, { useEffect } from 'react';

import { AppRouter } from './routes/AppRouter';
import { ThemeProvider } from './ThemeProvider';
import { SnackbarProvider } from 'notistack';
import { snackbarProviderConfig, SnackMessages } from '@shared/components/SnackMessages';
import { IAppStore } from '@shared/store/app';
import { useStores } from '@shared/helpers';
import { CssBaseline } from '@material-ui/core';

type Props = {
  history: History;
};

export const App: React.FC<Props> = observer(props => {
  const app = useStores().app as IAppStore;

  useEffect(
    () => {
      if (app) {
        app.connectAppToSocket();
      }
    },
    [],
  );

  return (
    <ThemeProvider>
      <CssBaseline />
      <SnackbarProvider {...snackbarProviderConfig}>
        <AppRouter history={props.history}/>
        <SnackMessages />
      </SnackbarProvider>
    </ThemeProvider>
  );
});
