using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements
{
    public class InventoryService
    {
        private readonly Container _stockPerLocationContainer;

        private const string StockPerLocationContainerName = "stock-per-location";

        public InventoryService(Database database)
        {
            _stockPerLocationContainer = database
                .CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/locationId").Result;
        }

        public async Task CalculateInventoryOnStorageLocations(StockByStorageLocation locationGroup)
        {
            var locationId = locationGroup.locationId;

            var result = await GetStockDocument<StockByStorageLocation>(locationId);

            if (result is null)
            {
                await CreateStockByLocations(_stockPerLocationContainer, locationId, locationGroup);

                return;
            }

            await UpdateStockByLocation(result, locationGroup, _stockPerLocationContainer);
        }

        public async Task<List<StockByStorageLocation>> GetInventoryOnStorageLocations()
        {
            var result = _stockPerLocationContainer.GetItemQueryIterator<StockByStorageLocation>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockByStorageLocation> response = await result.ReadNextAsync();

                if (response == null)
                {
                    break;
                }

                return response.ToList();
            }

            return null;
        }

        private async Task<ItemResponse<T>> GetStockDocument<T>(string documentId)
        {
            try
            {
                return await _stockPerLocationContainer.ReadItemAsync<T>(documentId, new PartitionKey(documentId));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"\nDocument with id: {documentId} was not found\n");
            }

            return null;
        }

        private async Task CreateStockByLocations(Container container, string locationId, StockByStorageLocation storageLocationGroup)
        {
            var storageLocationStock = new StockByStorageLocation
            {
                id = locationId,
                locationId = locationId,
                locationType = storageLocationGroup.locationType,
                checkpoints = storageLocationGroup.checkpoints
            };

            ItemRequestOptions requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};

            var response = await container.CreateItemAsync(storageLocationStock,
                new PartitionKey(locationId), requestOptions);

            Console.WriteLine($"\nRequest Charge for creating storage location: {response.RequestCharge}\n");
        }

        private async Task UpdateStockByLocation(ItemResponse<StockByStorageLocation> result, StockByStorageLocation locationGroup, Container container)
        {
            bool changesHappened;
            var locationId = locationGroup.locationId;

            var location = result.Resource;

            var checkpoints = location.checkpoints;

            if (checkpoints.Count == 0)
            {
                location.checkpoints.AddRange(locationGroup.checkpoints);
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

        private bool HandleChangesForLocation(StockByStorageLocation locationGroup, List<ArticleCheckpoint> checkpoints, StockByStorageLocation location)
        {
            var changesHappened = false;

            foreach (var checkpoint in checkpoints)
            {
                var newCheckpoint =
                    locationGroup.checkpoints.FirstOrDefault(x => x.articleId == checkpoint.articleId);

                if (newCheckpoint != null)
                {
                    checkpoint.quantity += newCheckpoint.quantity;
                    changesHappened = true;
                }

                locationGroup.checkpoints.Remove(newCheckpoint);
            }

            if (locationGroup.checkpoints.Count <= 0)
                return changesHappened;

            location.checkpoints.AddRange(locationGroup.checkpoints);

            return true;
        }
    }
}