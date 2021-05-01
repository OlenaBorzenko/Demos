import { IApiStore } from './Api';
import { INotificationsStore } from '@shared/store/notifications';

export type IStoresEnv = {
  localStorage: Storage;
  api: IApiStore;
  notifier: INotificationsStore;
};

export type IApiStoreEnv = {
  notifier: INotificationsStore;
};
