import { History } from 'history';
import { observer, inject } from 'mobx-react';
import React, { useEffect } from 'react';

import { ModalPlaceholder } from '@shared/Modal';
import { AppRouter } from './routes/AppRouter';
import { ThemeProvider } from './ThemeProvider';
import { SnackbarProvider } from 'notistack';
import { snackbarProviderConfig, SnackMessages } from '@shared/components/SnackMessages';
import { IAppStore, APP_INJECTION_KEY } from '@shared/store/app';

type Props = {
  history: History;
};

interface InjectedProps extends Props {
  app: IAppStore;
}

const AppComponent: React.FC<Props> = observer(props => {
  const app = (props as InjectedProps).app;

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
      <SnackbarProvider {...snackbarProviderConfig}>
        <AppRouter history={props.history}/>
        <ModalPlaceholder history={props.history}/>
        <SnackMessages />
      </SnackbarProvider>
    </ThemeProvider>
  );
});

export const App = inject(APP_INJECTION_KEY)(AppComponent);
