using System;
using System.Collections.Generic;
using System.Linq;
using Models.V1;
using Shared;

namespace MovementsV1Demo
{
    public static class ModelsHelper
    {
        public static ArticleMovement CreateArticleMovementModel()
        {
            // Getting random test data;
            var article = Helpers.GetItemByRandomIndex(TestData.Articles);
            var movement = Helpers.GetItemByRandomIndex(TestData.Movements);

            return new ArticleMovement
            {
                Id = Guid.NewGuid().ToString(),
                ArticleId = article.Id,
                ArticleName = article.Name,
                MovementType = movement.Type,
                FromLocationId = movement.From,
                ToLocationId = movement.To,
                TimeStamp = DateTimeOffset.Now
            };
        }

        public static StockCheckpointV1 CreateStockCheckpoint(List<AggregationV1> outboundAggregations, AggregationV1 inboundAggregation)
        {
            var fromItem = outboundAggregations
                .FirstOrDefault(x => x.LocationId == inboundAggregation.LocationId && x.ArticleId == inboundAggregation.ArticleId);

            return new StockCheckpointV1
            {
                Id = Guid.NewGuid().ToString(),
                ArticleId = inboundAggregation.ArticleId,
                ArticleName = inboundAggregation.ArticleName,
                Quantity = fromItem == null ? inboundAggregation.Count : inboundAggregation.Count - fromItem.Count,
                LocationId = inboundAggregation.LocationId,
                TimeStamp = DateTimeOffset.Now
            };
        }
    }
}