import { observer } from 'mobx-react';
import React, { useEffect, useState } from 'react';

import { useStyles } from './Home.style';
import { IHomeStore } from './store';
import { useStores } from '@shared/helpers/useStores';
import { Button, Container, Paper } from '@material-ui/core';
import { InventoryList } from 'Home/InventoryList';

interface Props {
}

export const Home: React.FC<Props> = observer(() => {
  const s = useStyles();
  const homeStore = useStores().home as IHomeStore;
  const [started, startInterval] = useState(false);

  useEffect(
    () => {
      if (homeStore) {
        homeStore.getStorageLocations();
      }

      let interval: any;

      if (started) {
        interval = setInterval(() => {
          homeStore.generateMovements();
          // The logic of changing counter value to come soon.
        }, 500);
      } else {
        clearInterval(interval);
      }

      return () => clearInterval(interval);
    },
    [started]
  );

  const changeInterval = () => {
    startInterval(!started);
  }

  return (
    <React.Fragment>
      <Container maxWidth={'lg'}>
        <Paper className={s.content}>
          <div className={s.buttonsGroup}>
            <Button variant={'outlined'} onClick={homeStore.generateMovements}>Create movements</Button>
            <Button variant={'outlined'} onClick={changeInterval}>
              {started ? 'Stop Interval' : 'Start Interval'}
            </Button>
          </div>
          <InventoryList locationsList={homeStore.locationsList} />
        </Paper>
      </Container>
    </React.Fragment>
  );
});
