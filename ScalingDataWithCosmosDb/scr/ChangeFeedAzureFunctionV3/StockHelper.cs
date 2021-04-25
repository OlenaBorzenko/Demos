using System.Collections.Generic;
using System.Linq;
using Models.V2;
using Shared;

namespace ChangeFeedAzureFunctionV3
{
    public static class StockHelper
    {
        public static (IEnumerable<StockByLocationV2>, IEnumerable<StockByArticleV2>) GroupInboundOutboundStock(IReadOnlyCollection<ArticleMovement> changes)
        {
            var fromLocationItems = changes
                .GroupBy(c => c.FromLocationId)
                .SelectMany(x => x.Select((item, key) => new
                {
                    LocationId = item.FromLocationId,
                    item.ArticleId,
                    item.Id,
                    item.ArticleName,
                    item.MovementType,
                    QuantityChange = -1
                })).ToList();

            var toLocationItems = changes
                .GroupBy(c => c.ToLocationId)
                .SelectMany(x => x.Select((item, key) => new
                {
                    LocationId = item.ToLocationId,
                    item.ArticleId,
                    item.Id,
                    item.ArticleName,
                    item.MovementType,
                    QuantityChange = 1
                })).ToList();

            fromLocationItems.AddRange(toLocationItems);

            var summaryByLocation = fromLocationItems.GroupBy(x => x.LocationId)
                .Select((item, key) => new StockByLocationV2
                {
                    LocationId = item.Key,
                    LocationType = item.First().MovementType,
                    Checkpoints = item
                        .GroupBy(x => x.ArticleId)
                        .Select((articles, _) =>
                        {
                            var sum = articles.Sum(a => a.QuantityChange);

                            return new ArticleCheckpointV2
                            {
                                ArticleId = articles.First().ArticleId,
                                Quantity = sum,
                                ArticleName = articles.First().ArticleName
                            };
                        }).ToList()
                });

            var summaryByArticle = fromLocationItems.GroupBy(x => x.ArticleId)
                .Select((item, key) => new StockByArticleV2
                {
                    ArticleId = item.Key,
                    ArticleName = item.First().ArticleName,
                    Checkpoints = item
                        .GroupBy(x => x.LocationId)
                        .Select((articles, _) =>
                        {
                            var sum = articles.Sum(a => a.QuantityChange);

                            return new LocationCheckpointV2
                            {
                                LocationId = articles.First().LocationId,
                                Quantity = sum < 0 ? 0 : sum,
                            };
                        }).ToList()
                });

            return (summaryByLocation, summaryByArticle);
        }

    }
}