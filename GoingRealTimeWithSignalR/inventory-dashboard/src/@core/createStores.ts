import { getStorage } from '@utils/getStorage';
import { ApiStore } from './Api';
import { IStoresEnv } from './storesEnv';
import { NotificationsStore } from '@shared/store/notifications';
import { AppStore, APP_INJECTION_KEY, HOME_INJECTION_KEY } from '@shared/store/app';
import { HomeStore } from 'Home/store/HomeStore';

export const initializeDependenciesAndCreateStores = async () => {
  const notifier = NotificationsStore.create();
  const api = ApiStore.create({}, { notifier });

  const env: IStoresEnv = {
    api,
    notifier,
    localStorage: getStorage(),
  };

  return ({
    [APP_INJECTION_KEY]: AppStore.create({}, env),
    [HOME_INJECTION_KEY]: HomeStore.create({}, env),
  });
};

export type Stores = UnwrapAsyncFnReturn<typeof initializeDependenciesAndCreateStores>;
