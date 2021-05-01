using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Models.V2;

namespace ChangeFeedAzureFunctionV3
{
    public class InventoryService
    {
        private readonly Container _stockPerLocationContainer;
        private readonly Container _stockPerArticleContainer;

        private const string StockPerLocationContainerName = "stock-per-location";
        private const string StockPerArticleContainerName = "stock-per-article";

        public InventoryService(Database database)
        {
            _stockPerLocationContainer = database
                .CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/LocationId").Result;
            _stockPerArticleContainer = database
                .CreateContainerIfNotExistsAsync(StockPerArticleContainerName, "/ArticleId").Result;
        }

        public async Task CalculateInventoryForArticles(StockByArticleV2 articleGroup)
        {
            var articleId = articleGroup.ArticleId;

            var result = await GetStockDocument<StockByArticleV2>(_stockPerArticleContainer, articleId);

            if (result is null)
            {
                await CreateStockByArticles(_stockPerArticleContainer, articleId, articleGroup);

                return;
            }

            await UpdateStockByArticles(result, articleGroup, _stockPerArticleContainer);
        }

        public async Task CalculateInventoryOnStorageLocations(StockByLocationV2 locationGroup)
        {
            var locationId = locationGroup.LocationId;

            var result = await GetStockDocument<StockByLocationV2>(_stockPerLocationContainer, locationId);

            if (result is null)
            {
                await CreateStockByLocations(_stockPerLocationContainer, locationId, locationGroup);

                return;
            }

            await UpdateStockByLocation(result, locationGroup, _stockPerLocationContainer);
        }

        private async Task<ItemResponse<T>> GetStockDocument<T>(Container container, string documentId)
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

        private async Task CreateStockByArticles(Container container, string articleId, StockByArticleV2 articleGroup)
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

        private async Task CreateStockByLocations(Container container, string locationId, StockByLocationV2 locationGroup)
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

        private async Task UpdateStockByArticles(ItemResponse<StockByArticleV2> result, StockByArticleV2 articleGroup, Container container)
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

        private async Task UpdateStockByLocation(ItemResponse<StockByLocationV2> result, StockByLocationV2 locationGroup, Container container)
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

        private bool HandleChangesForLocation(StockByLocationV2 locationGroup, List<ArticleCheckpointV2> checkpoints, StockByLocationV2 location)
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

        private bool HandleChangesForArticles(StockByArticleV2 articleGroup, List<LocationCheckpointV2> checkpoints, StockByArticleV2 article)
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
    }
}