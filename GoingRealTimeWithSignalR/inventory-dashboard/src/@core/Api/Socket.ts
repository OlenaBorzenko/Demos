import * as signalR from '@aspnet/signalr';
import { Instance, types, getEnv } from 'mobx-state-tree';
import { Subject } from 'rxjs/internal/Subject';

import { SocketEvent } from '@shared/enums';
import { IStoresEnv } from '@core/storesEnv';
import { Observable } from 'rxjs';

type SocketChannel = { [key in SocketEvent]: Subject<any> };

export const Socket = types
  .model('socket', {})
  .actions(self => {
    let connection: signalR.HubConnection;
    const socketChannels: Partial<SocketChannel> = {};
    const maxAttempts = 5;
    let attemptsCount = 0;

    return {
      connectToSocket () {
        connection = new signalR.HubConnectionBuilder()
          .withUrl('http://localhost/api')
          .configureLogging(signalR.LogLevel.Error)
          .build();

        const { notifier } = getEnv<IStoresEnv>(self);

        async function start () {
          const delayTime = 3000;
          try {
            await connection.start();
            attemptsCount = 0;
          } catch (err) {
            attemptsCount++;
            if (attemptsCount === 1) {
              notifier.addNotification({
                message: 'Connection failed. Please try again', options: { persist: true },
              });
            }
            if (attemptsCount < maxAttempts) {
              setTimeout(start, delayTime);
            } else {
              notifier.addNotification({
                message: 'There is a problem with getting update messages.',
              });
            }
          }
        }

        connection.onclose(() => {
          start();
        });

        start();
      },
      getStream<T> (eventName: SocketEvent): Observable<Subject<T>> {
        let subscription = socketChannels[eventName];

        if (subscription) {
          return subscription;
        }

        subscription = new Subject();
        socketChannels[eventName] = subscription;
        if (connection) {
          connection.on(eventName, (data: T) => {

            return subscription && subscription.next(data);
          });
        }

        return subscription.asObservable();
      },
    };
  });

export type ISocketStore = Instance<typeof Socket>;
