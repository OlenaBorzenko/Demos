import { getEnv, Instance, types } from 'mobx-state-tree';

import { IStoresEnv } from '@core/storesEnv';
import { IHomeStore } from 'Home/store';

export const APP_INJECTION_KEY = 'app';
export const HOME_INJECTION_KEY = 'homeStore';

export interface IInjectedAppStore {
  [APP_INJECTION_KEY]: IAppStore;
  [HOME_INJECTION_KEY]: IHomeStore;
}

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
