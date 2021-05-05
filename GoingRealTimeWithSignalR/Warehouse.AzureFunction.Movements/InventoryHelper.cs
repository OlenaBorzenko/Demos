using System.Collections.Generic;
using System.Linq;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements
{
    public static class StockHelper
    {
        public static IEnumerable<StockByStorageLocation> GroupInboundOutboundStock(IReadOnlyCollection<ArticleMovement> changes)
        {
            var fromLocationItems = changes
                .GroupBy(c => c.fromLocationId)
                .SelectMany(x => x.Select((item, key) => new
                {
                    LocationId = item.fromLocationId,
                    ArticleId = item.articleId,
                    item.id,
                    ArticleName = item.articleName,
                    MovementType = item.movementType,
                    QuantityChange = -1
                })).ToList();

            var toLocationItems = changes
                .GroupBy(c => c.toLocationId)
                .SelectMany(x => x.Select((item, key) => new
                {
                    LocationId = item.toLocationId,
                    ArticleId = item.articleId,
                    item.id,
                    ArticleName = item.articleName,
                    MovementType = item.movementType,
                    QuantityChange = 1
                })).ToList();

            fromLocationItems.AddRange(toLocationItems);

            var summaryByLocation = fromLocationItems.GroupBy(x => x.LocationId)
                .Select((item, key) => new StockByStorageLocation
                {
                    locationId = item.Key,
                    locationType = item.First().MovementType,
                    checkpoints = item
                        .GroupBy(x => x.ArticleId)
                        .Select((articles, _) =>
                        {
                            var sum = articles.Sum(a => a.QuantityChange);

                            return new ArticleCheckpoint
                            {
                                articleId = articles.First().ArticleId,
                                quantity = sum,
                                articleName = articles.First().ArticleName
                            };
                        }).ToList()
                });

            return summaryByLocation;
        }
    }
}