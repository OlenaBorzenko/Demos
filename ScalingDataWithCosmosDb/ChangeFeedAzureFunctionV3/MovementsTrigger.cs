using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models.V2;
using Newtonsoft.Json;
using Shared;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace ChangeFeedAzureFunctionV3
{
    public static class MovementsTrigger
    {
        #region PrivateFields

        private static readonly string DatabaseName = Environment.GetEnvironmentVariable("CosmosDBDatabase");
        private static readonly string Connection = Environment.GetEnvironmentVariable("CosmosDBConnection");
        private const string StockPerLocationContainerName = "stock-per-location";
        private const string StockPerArticleContainerName = "stock-per-article";

        private static readonly CosmosClient Client = new CosmosClient(Connection);
        private static readonly Container StockPerLocationContainer = Client
            .GetDatabase(DatabaseName)
            .CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/LocationId").Result;

        private static readonly Container StockPerArticleContainer = Client
            .GetDatabase(DatabaseName)
            .CreateContainerIfNotExistsAsync(StockPerArticleContainerName, "/ArticleId").Result;

        #endregion

        [FunctionName("MovementsTrigger")]
        public static async Task RunAsync([CosmosDBTrigger(
                databaseName: "%CosmosDBDatabase%",
                collectionName: "%CosmosDBContainer%",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "movements-leases-containerV3",
                CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {

                var movements = input.
                    Select(item => JsonConvert.DeserializeObject<ArticleMovement>(item.ToString())).ToList();

                await HandleMovementsChanges(movements);
            }
        }

        private static async Task HandleMovementsChanges(IReadOnlyList<ArticleMovement> changes)
        {
            Console.WriteLine(changes.Count + " Change(s) Received");

            List<Task> tasks = new List<Task>();

            var result = GroupInboundOutboundStock(changes);

            foreach (var locationGroup in result.Item1)
            {
                tasks.Add(CalculateInventoryOnStorageLocations(locationGroup));
            }
            foreach (var articleGroup in result.Item2)
            {
                tasks.Add(CalculateInventoryForArticles(articleGroup));
            }

            await Task.WhenAll(tasks);
        }

        private static async Task CalculateInventoryForArticles(StockByArticleV2 articleGroup)
        {
            var articleId = articleGroup.ArticleId;

            var result = await GetStockDocument<StockByArticleV2>(StockPerArticleContainer, articleId);

            if (result is null)
            {
                await CreateStockByArticles(articleId, articleGroup);

                return;
            }

            await UpdateStockByArticles(result, articleGroup);
        }

        private static async Task CalculateInventoryOnStorageLocations(StockByLocationV2 locationGroup)
        {
            var locationId = locationGroup.LocationId;

            var result = await GetStockDocument<StockByLocationV2>(StockPerLocationContainer, locationId);

            if (result is null)
            {
                await CreateStockByLocations(locationId, locationGroup);

                return;
            }

            await UpdateStockByLocation(result, locationGroup);
        }

        #region HelperMethods

        private static (IEnumerable<StockByLocationV2>, IEnumerable<StockByArticleV2>) GroupInboundOutboundStock(IReadOnlyCollection<ArticleMovement> changes)
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

        private static async Task<ItemResponse<T>> GetStockDocument<T>(Container container, string documentId)
        {
            try
            {
                return await container.ReadItemAsync<T>(documentId, new PartitionKey(documentId));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"\nDocument with id: {documentId} was not found\n");
            }

            return null;
        }

        private static async Task CreateStockByArticles(string articleId, StockByArticleV2 articleGroup)
        {
            var articleStock = new StockByArticleV2
            {
                Id = articleId,
                ArticleId = articleId,
                ArticleName = articleGroup.ArticleName,
                Checkpoints = articleGroup.Checkpoints
            };

            ItemRequestOptions requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};

            var response = await StockPerArticleContainer.CreateItemAsync(articleStock,
                new PartitionKey(articleId), requestOptions);

            Console.WriteLine($"\nRequest Charge for creating storage location: {response.RequestCharge}\n");
        }

        private static async Task CreateStockByLocations(string locationId, StockByLocationV2 locationGroup)
        {
            var storageLocationStock = new StockByLocationV2
            {
                Id = locationId,
                LocationId = locationId,
                LocationType = locationGroup.LocationType,
                Checkpoints = locationGroup.Checkpoints
            };

            ItemRequestOptions requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};

            var response = await StockPerLocationContainer.CreateItemAsync(storageLocationStock,
                new PartitionKey(locationId), requestOptions);

            Console.WriteLine($"\nRequest Charge for creating storage location: {response.RequestCharge}\n");
        }

        private static async Task UpdateStockByArticles(ItemResponse<StockByArticleV2> result, StockByArticleV2 articleGroup)
        {
            bool changesHappened;
            var articleId = articleGroup.ArticleId;

            var article = result.Resource;

            var checkpoints = article.Checkpoints;

            if (checkpoints.Count == 0)
            {
                article.Checkpoints.AddRange(articleGroup.Checkpoints);
                changesHappened = true;
            }
            else
            {
                changesHappened = HandleChangesForArticles(articleGroup, checkpoints, article);
            }

            if (changesHappened)
            {
                var responseForUpdate = await StockPerArticleContainer.ReplaceItemAsync(
                    partitionKey: new PartitionKey(articleId),
                    id: articleId,
                    item: article);

                Console.WriteLine($"\nRequest Charge for updating storage location: {responseForUpdate.RequestCharge}\n");
            }
            else
            {
                Console.WriteLine($"\nNo changes were made\n");
            }
        }

        private static async Task UpdateStockByLocation(ItemResponse<StockByLocationV2> result, StockByLocationV2 locationGroup)
        {
            bool changesHappened;
            var locationId = locationGroup.LocationId;

            var location = result.Resource;

            var checkpoints = location.Checkpoints;

            if (checkpoints.Count == 0)
            {
                location.Checkpoints.AddRange(locationGroup.Checkpoints);
                changesHappened = true;
            }
            else
            {
                changesHappened = HandleChangesForLocation(locationGroup, checkpoints, location);
            }

            if (changesHappened)
            {
                var responseForUpdate = await StockPerLocationContainer.ReplaceItemAsync(
                    partitionKey: new PartitionKey(locationId),
                    id: locationId,
                    item: location);

                Console.WriteLine($"\nRequest Charge for updating storage location: {responseForUpdate.RequestCharge}\n");
            }
            else
            {
                Console.WriteLine($"\nNo changes were made\n");
            }
        }

        private static bool HandleChangesForLocation(StockByLocationV2 locationGroup, List<ArticleCheckpointV2> checkpoints, StockByLocationV2 location)
        {
            var changesHappened = false;

            foreach (var checkpoint in checkpoints)
            {
                var newCheckpoint =
                    locationGroup.Checkpoints.FirstOrDefault(x => x.ArticleId == checkpoint.ArticleId);

                if (newCheckpoint != null)
                {
                    checkpoint.Quantity += newCheckpoint.Quantity;
                    changesHappened = true;
                }

                locationGroup.Checkpoints.Remove(newCheckpoint);
            }

            if (locationGroup.Checkpoints.Count > 0)
            {
                location.Checkpoints.AddRange(locationGroup.Checkpoints);
                changesHappened = true;
            }

            return changesHappened;
        }

        private static bool HandleChangesForArticles(StockByArticleV2 articleGroup, List<LocationCheckpointV2> checkpoints, StockByArticleV2 article)
        {
            var changesHappened = false;

            foreach (var checkpoint in checkpoints)
            {
                var newCheckpoint =
                    articleGroup.Checkpoints.FirstOrDefault(x => x.LocationId == checkpoint.LocationId);

                if (newCheckpoint != null)
                {
                    checkpoint.Quantity += newCheckpoint.Quantity;
                    changesHappened = true;
                }

                articleGroup.Checkpoints.Remove(newCheckpoint);
            }

            if (articleGroup.Checkpoints.Count > 0)
            {
                article.Checkpoints.AddRange(articleGroup.Checkpoints);
                changesHappened = true;
            }

            return changesHappened;
        }

        #endregion
    }
}