using System.Collections.Generic;
using System.Linq;
using Shared;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements
{
    public static class StockHelper
    {
        public static IEnumerable<StockByStorageLocation> GroupInboundOutboundStock(IReadOnlyCollection<ArticleMovement> changes)
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
                .Select((item, key) => new StockByStorageLocation
                {
                    LocationId = item.Key,
                    LocationType = item.First().MovementType,
                    Checkpoints = item
                        .GroupBy(x => x.ArticleId)
                        .Select((articles, _) =>
                        {
                            var sum = articles.Sum(a => a.QuantityChange);

                            return new ArticleCheckpoint
                            {
                                ArticleId = articles.First().ArticleId,
                                Quantity = sum,
                                ArticleName = articles.First().ArticleName
                            };
                        }).ToList()
                });

            return summaryByLocation;
        }
    }
}