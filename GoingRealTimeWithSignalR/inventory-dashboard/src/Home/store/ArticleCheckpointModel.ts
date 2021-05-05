import { types, Instance } from 'mobx-state-tree';

export const ArticleCheckpointModel = types
  .model({
    articleId: types.optional(types.string, ''),
    articleName: types.optional(types.string, ''),
    quantity: types.optional(types.number, 0),
    timeStamp: types.optional(types.string, ''),
  });

export type IArticleCheckpointModel = Instance<typeof ArticleCheckpointModel>;
