import { applySnapshot, cast, flow, getEnv, Instance, types } from 'mobx-state-tree';
import { DataState, SocketEvent } from '@shared/enums';
import { IStoresEnv } from '@core/storesEnv';
import { IStorageLocationModel, StorageLocationModel } from 'Home/store/StorageLocationModel';
import { ApiResponse, DataStateStore } from '@core/Api';
import { generateMovements, getStorageLocations } from 'Home/endpoints';
import _ from 'lodash';

export const HomeStore = types
  .model({
    sampleText: types.optional(types.string, 'This is my initial state'),
    storageLocations: types.optional(types.array(StorageLocationModel), []),
  })
  .volatile(() => ({
    loadingDataState: DataStateStore.create({ state: DataState.initial }),
  }))
  .views(self => ({
    get locationsList () {
      return self.storageLocations.sort((a, b) => (a.locationId > b.locationId) ? 1 : -1);
    },
  }))
  .actions(self => {
    return ({
      handleStockChange (stockData: any) {

        self.storageLocations.forEach((location: any) => {
          const locationIndex = stockData.findIndex((item: any) => item.id === location.id);

          if (locationIndex >= 0) {
            location.updateCheckpoints(stockData[locationIndex].checkpoints)
            stockData.splice(locationIndex, 1);
          }
        })

        if (!_.isEmpty(stockData)) {
          self.storageLocations.push(...cast(stockData));
        }
      },
      generateMovements: flow(function* ()  {
        self.loadingDataState.setLoading();
        const env = getEnv<IStoresEnv>(self);

        const response: ApiResponse = yield generateMovements(env.api, {
          payload: {},
          errorHandlers: { DEFAULT: 'FailedToGenerateMovements' },
        });

        if (response.error) {
          self.loadingDataState.setError();

          return;
        }

        self.loadingDataState.setDone();
      }),
      getStorageLocations: flow(function* ()  {
        self.loadingDataState.setLoading();
        const env = getEnv<IStoresEnv>(self);

        const response: ApiResponse<IStorageLocationModel[]> = yield getStorageLocations(env.api, {
          payload: {},
          errorHandlers: { DEFAULT: 'FailedToGetStorageLocations' },
        });

        if (response.error) {
          self.loadingDataState.setError();

          return;
        }

        applySnapshot(self.storageLocations, response.data || []);

        self.loadingDataState.setDone();
      }),
    });
  })
  .actions(self => ({
    afterCreate () {
      const { api } = getEnv<IStoresEnv>(self);

      api.connectToSocket();
      const stream = api.getStream(SocketEvent.stockPerLocationChanged);

      stream.subscribe((data: any) => {
        self.handleStockChange(data);
      });
    },
  }));

export type IHomeStore = Instance<typeof HomeStore>;
