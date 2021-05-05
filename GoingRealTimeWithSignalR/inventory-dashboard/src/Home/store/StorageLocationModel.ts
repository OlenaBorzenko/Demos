import { types, Instance, applySnapshot, cast } from 'mobx-state-tree';
import { ArticleCheckpointModel } from 'Home/store/ArticleCheckpointModel';
import _ from 'lodash';

export const StorageLocationModel = types
  .model({
    id: types.optional(types.string, ''),
    locationId: types.optional(types.string, ''),
    locationType: types.optional(types.string, ''),
    checkpoints: types.optional(types.array(ArticleCheckpointModel), []),
    expanded: types.optional(types.boolean, true),
  })
  .views(self => ({
    get title () {
      return `Storage location: ${self.locationId} - ${self.locationType}. `;
    },
  }))
  .actions(self => {
    return ({
      handleExpand () {
        self.expanded = !self.expanded;
      },
      updateCheckpoints (newCheckpoints: any) {
        self.checkpoints.forEach((checkpoint: any) => {
          const checkpointIndex = newCheckpoints.findIndex((item: any) => item.articleId === checkpoint.articleId);

          if (checkpointIndex >= 0) {
            applySnapshot(checkpoint, newCheckpoints[checkpointIndex]);
            newCheckpoints.splice(checkpointIndex, 1);
          }
        })

        if (!_.isEmpty(newCheckpoints)) {
          self.checkpoints.push(...cast(newCheckpoints));
        }
      },
    })
  });

export type IStorageLocationModel = Instance<typeof StorageLocationModel>;
