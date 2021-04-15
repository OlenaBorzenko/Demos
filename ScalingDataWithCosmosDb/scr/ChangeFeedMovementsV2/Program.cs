﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private static readonly string DatabaseName = Configuration["DatabaseName"];
        private static readonly string Connection = Configuration["CosmosDBConnection"];

        private const string MovementContainerName = "movements";
        private const string StockPerLocationContainerName = "stock-per-location";
        private const string StockPerArticleContainerName = "stock-per-article";

        private static readonly CosmosClient Client = new(Connection, new CosmosClientOptions { AllowBulkExecution = true });
        private static readonly Database Database = Client.GetDatabase(DatabaseName);

        public static async Task Main(string[] args)
        {
            ContainerProperties leaseContainerProperties = new ContainerProperties("movements-leases-containerV2", "/id");
            Container leaseContainer = await Database
                .CreateContainerIfNotExistsAsync(leaseContainerProperties, throughput: 400);

            await Database.CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/LocationId");
            await Database.CreateContainerIfNotExistsAsync(StockPerArticleContainerName, "/ArticleId");

            var processor = Database
                .GetContainer(MovementContainerName)
                .GetChangeFeedProcessorBuilder<ArticleMovement>("MovementProcessor", HandleMovementsChanges)
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

        private static async Task HandleMovementsChanges(IReadOnlyCollection<ArticleMovement> changes, CancellationToken cancellationToken)
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
            Container container = Database.GetContainer(StockPerArticleContainerName);

            var articleId = articleGroup.ArticleId;

            var result = await GetStockDocument<StockByArticleV2>(container, articleId);

            if (result is null)
            {
                await CreateStockByArticles(container, articleId, articleGroup);

                return;
            }

            await UpdateStockByArticles(result, articleGroup, container);
        }

        private static async Task CalculateInventoryOnStorageLocations(StockByLocationV2 locationGroup)
        {
            Container container = Database.GetContainer(StockPerLocationContainerName);

            var locationId = locationGroup.LocationId;

            var result = await GetStockDocument<StockByLocationV2>(container, locationId);

            if (result is null)
            {
                await CreateStockByLocations(container, locationId, locationGroup);

                return;
            }

            await UpdateStockByLocation(result, locationGroup, container);
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

        private static async Task CreateStockByArticles(Container container, string articleId, StockByArticleV2 articleGroup)
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
        }

        private static async Task CreateStockByLocations(Container container, string locationId, StockByLocationV2 locationGroup)
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
        }

        private static async Task UpdateStockByArticles(ItemResponse<StockByArticleV2> result, StockByArticleV2 articleGroup, Container container)
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

        private static async Task UpdateStockByLocation(ItemResponse<StockByLocationV2> result, StockByLocationV2 locationGroup, Container container)
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