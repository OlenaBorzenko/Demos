import { types } from 'mobx-state-tree';

import { DataState } from '@shared/enums';

export const createDataStateType = (initial = DataState.pending) =>
  types.optional(types.enumeration(Object.keys(DataState) as DataState[]), initial);
