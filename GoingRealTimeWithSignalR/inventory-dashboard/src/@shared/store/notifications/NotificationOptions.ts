import { Instance, SnapshotIn, types } from 'mobx-state-tree';

export const NotificationOptions = types.model('NotificationOptions', {
  persist: types.boolean,
});

export const defaultOptions = { persist: false };
export type INotificationOptions = Instance<typeof NotificationOptions>;
export type NotificationOptionsData = SnapshotIn<INotificationOptions>;
