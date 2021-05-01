import { Instance, SnapshotIn, types } from 'mobx-state-tree';
import { defaultOptions, NotificationOptions } from './NotificationOptions';

export const Notification = types.model('Notification', {
  message: types.identifier,
  options: types.optional(NotificationOptions, defaultOptions),
});

export type INotification = Instance<typeof Notification>;
export type NotificationData = SnapshotIn<INotification>;
