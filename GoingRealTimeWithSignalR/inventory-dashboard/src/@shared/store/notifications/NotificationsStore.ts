import { Instance, types } from 'mobx-state-tree';

import { Notification, NotificationData } from './Notification';

export const NotificationsStore = types
  .model({
    notifications: types.map(Notification),
  })
  .actions(self => ({
    removeNotification: (notification: NotificationData) => {
      self.notifications.delete(notification.message);
    },
    addNotification: (notification: NotificationData) => {
      self.notifications.has(notification.message)
        ? self.notifications.replace(notification)
        : self.notifications.put(Notification.create(notification));
    },
  }));

export const notifier = NotificationsStore.create();

export type INotificationsStore = Instance<typeof NotificationsStore>;
