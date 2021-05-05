import React from 'react';
import { AppBar, Toolbar, Typography } from '@material-ui/core';
import { useStyles } from './Header.style';

interface Props {
}

export const Header: React.FC<Props> = () => {
  const s = useStyles();

  return (
    <div className={s.root}>
      <AppBar position="fixed" color={'secondary'} >
        <Toolbar>
          <Typography variant="h6" noWrap>
            Inventory Monitoring with Azure SignalR
          </Typography>
        </Toolbar>
      </AppBar>

    </div>
  );
};

