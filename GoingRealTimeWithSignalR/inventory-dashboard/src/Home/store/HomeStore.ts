import { Instance, types } from 'mobx-state-tree';

export const HomeStore = types
  .model({
    sampleText: types.optional(types.string, 'This is my initial state'),
  })
  .views(self => ({
    get capitilized () {
      return self.sampleText.toUpperCase();
    },
  }))
  .actions(self => {
    return ({
      invokeTestChange () {
        self.sampleText = 'This is my modified state.';
      },
    });
  });

export type IHomeStore = Instance<typeof HomeStore>;
