import { getStorage } from '@utils/getStorage';
import { ApiStore } from './Api';
import { IStoresEnv } from './storesEnv';
import { NotificationsStore } from '@shared/store/notifications';
import { AppStore } from '@shared/store/app';
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
    app: AppStore.create({}, env),
    home: HomeStore.create({}, env),
  });
};

export type Stores = UnwrapAsyncFnReturn<typeof initializeDependenciesAndCreateStores>;
