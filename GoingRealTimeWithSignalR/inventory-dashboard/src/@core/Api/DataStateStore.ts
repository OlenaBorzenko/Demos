/* eslint-disable @typescript-eslint/no-unused-vars */
import { types } from 'mobx-state-tree';

import { DataState } from '@shared/enums';

export const DataStateStore = types
  .model({
    state: types.optional(types.enumeration(Object.keys(DataState) as DataState[]), DataState.initial),
  })
  .views(self => ({
    get isLoading () {
      return self.state === DataState.pending;
    },
  }))
  .actions(self => {
    return ({
      setLoading () {
        self.state = DataState.pending
      },
      setDone () {
        self.state = DataState.done
      },
      setError () {
        self.state = DataState.error
      },
    });
  });
