import React from 'react';
import { AppBar, Toolbar } from '@material-ui/core';
import { observer, inject } from 'mobx-react';
import classNames from 'classnames';

import { Logo } from '../Logo';
import { useStyles } from './Header.style';
import { IInjectedAppStore, APP_INJECTION_KEY } from '@shared/store/app';

interface Props {
}

interface InjectedProps extends Props, IInjectedAppStore {
}

export const HeaderComponent: React.FC<Props> = observer(props => {
  const s = useStyles();
  const app = (props as InjectedProps).app;

  const info = app.info;

  return (
    <AppBar position="static" elevation={0} className={s.header}>
      <Toolbar disableGutters={true} className={classNames(s.toolbar, s.centeredContent)}>
        <Logo />
        {info}
      </Toolbar>
    </AppBar>
  );
});

export const Header = inject(APP_INJECTION_KEY)(HeaderComponent);
