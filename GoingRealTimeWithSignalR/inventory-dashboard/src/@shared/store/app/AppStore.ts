import { getEnv, Instance, types } from 'mobx-state-tree';

import { IStoresEnv } from '@core/storesEnv';

export const AppStore = types
  .model('AppStore', {})
  .views(self => {
    const { notifier } = getEnv<IStoresEnv>(self);

    return ({
      get notifications () { return notifier.notifications; },
      get info () {
        return 'App info';
      },
    });
  })
  .actions(self => {
    const { notifier, api } = getEnv<IStoresEnv>(self);

    return ({
      addNotification: notifier.addNotification,
      removeNotification: notifier.removeNotification,
      connectAppToSocket: api.connectToSocket,
    });
  });

export type IAppStore = Instance<typeof AppStore>;
