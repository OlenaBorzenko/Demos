using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Models.V2;
using Shared;

namespace ChangeFeedMovementsV2
{
    static class Program
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private static readonly string Endpoint = Configuration["EndPointUrl"];
        private static readonly string DatabaseName = Configuration["DatabaseName"];
        private static readonly string Key = Configuration["AuthorizationKey"];

        private const string MovementContainerName = "movements";
        private const string StockPerLocationContainerName = "stock-per-location";
        private const string StockPerArticleContainerName = "stock-per-article";

        private static readonly CosmosClient _client = new(Endpoint, Key, new CosmosClientOptions { AllowBulkExecution = true });
        private static readonly Database _database = _client.GetDatabase(DatabaseName);

        public static async Task Main(string[] args)
        {
            ContainerProperties leaseContainerProperties = new ContainerProperties("movements-leases-container", "/id");
            Container leaseContainer = await _database
                .CreateContainerIfNotExistsAsync(leaseContainerProperties, throughput: 400);

            var processor = _database
                .GetContainer(MovementContainerName)
                .GetChangeFeedProcessorBuilder<ArticleMovement>("MovementProcessor", HandleProcessorChanges)
                .WithInstanceName("ChangeFeedProductCategories")
                .WithLeaseContainer(leaseContainer)
                .Build();

            Console.WriteLine("Starting Change Feed Processor...");
            await processor.StartAsync();
            Console.WriteLine("Change Feed Processor started.");

            Console.WriteLine("Press any key to stop the processor...");
            Console.ReadKey();

            Console.WriteLine("Stopping Change Feed Processor");

            await processor.StopAsync();
        }

        private static async Task HandleProcessorChanges(IReadOnlyCollection<ArticleMovement> changes, CancellationToken cancellationToken)
        {
            Console.WriteLine(changes.Count + " Change(s) Received");

            List<Task> tasks = new List<Task>();

            var result = GroupInboundOutboundStock(changes);

            foreach (var locationGroup in result.Item1)
            {
                tasks.Add(UpdateStorageLocationQuantity(locationGroup));
            }
            foreach (var articleGroup in result.Item2)
            {
                tasks.Add(UpdateArticlesQuantity(articleGroup));
            }

            await Task.WhenAll(tasks);
        }

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

        private static async Task UpdateStorageLocationQuantity(StockByLocationV2 locationGroup)
        {
            Container container = _database.GetContainer(StockPerLocationContainerName);

            ItemResponse<StockByLocationV2> result = null;

            var locationId = locationGroup.LocationId;

            try
            {
                result = await container.ReadItemAsync<StockByLocationV2>(locationId,
                    new PartitionKey(locationId));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"\nDocument with location id: {locationId} was not found\n");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"\nException: {exc.Message}\n");
            }

            if (result == null)
            {
                var storageLocationStock = new StockByLocationV2
                {
                    Id = locationId,
                    LocationId = locationId,
                    LocationType = locationGroup.LocationType,
                    Checkpoints = locationGroup.Checkpoints
                };

                ItemRequestOptions requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};

                var response = await container.CreateItemAsync(storageLocationStock,
                    new PartitionKey(locationId), requestOptions);

                Console.WriteLine($"\nRequest Charge for creating storage location: {response.RequestCharge}\n");

                return;
            }

            var changesHappened = false;

            var location = result.Resource;

            var checkpoints = location.Checkpoints;

            if (checkpoints.Count == 0)
            {
                location.Checkpoints.AddRange(locationGroup.Checkpoints);
                changesHappened = true;
            }
            else
            {
                changesHappened = HandleChangesForLocation(locationGroup, checkpoints, changesHappened, location);
            }

            if (changesHappened)
            {
                var responseForUpdate = await container.ReplaceItemAsync(
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

         private static async Task UpdateArticlesQuantity(StockByArticleV2 articleGroup)
        {
            Container container = _database.GetContainer(StockPerArticleContainerName);

            ItemResponse<StockByArticleV2> result = null;

            var articleId = articleGroup.ArticleId;

            try
            {
                result = await container.ReadItemAsync<StockByArticleV2>(articleId,
                    new PartitionKey(articleId));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"\nDocument with location id: {articleId} was not found\n");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"\nException: {exc.Message}\n");
            }

            if (result == null)
            {
                var articleStock = new StockByArticleV2
                {
                    Id = articleId,
                    ArticleId = articleId,
                    ArticleName = articleGroup.ArticleName,
                    Checkpoints = articleGroup.Checkpoints
                };

                ItemRequestOptions requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};

                var response = await container.CreateItemAsync(articleStock,
                    new PartitionKey(articleId), requestOptions);

                Console.WriteLine($"\nRequest Charge for creating storage location: {response.RequestCharge}\n");

                return;
            }

            var changesHappened = false;

            var article = result.Resource;

            var checkpoints = article.Checkpoints;

            if (checkpoints.Count == 0)
            {
                article.Checkpoints.AddRange(articleGroup.Checkpoints);
                changesHappened = true;
            }
            else
            {
                changesHappened = HandleChangesForArticles(articleGroup, checkpoints, changesHappened, article);
            }

            if (changesHappened)
            {
                var responseForUpdate = await container.ReplaceItemAsync(
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

        private static bool HandleChangesForLocation(StockByLocationV2 locationGroup, List<ArticleCheckpointV2> checkpoints, bool changesHappened,
            StockByLocationV2 location)
        {
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

        private static bool HandleChangesForArticles(StockByArticleV2 articleGroup, List<LocationCheckpointV2> checkpoints, bool changesHappened,
            StockByArticleV2 article)
        {
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
    }
}