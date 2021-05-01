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
                .CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/LocationId").Result;
        }

        public async Task CalculateInventoryOnStorageLocations(StockByStorageLocation locationGroup)
        {
            var locationId = locationGroup.LocationId;

            var result = await GetStockDocument<StockByStorageLocation>(_stockPerLocationContainer, locationId);

            if (result is null)
            {
                await CreateStockByLocations(_stockPerLocationContainer, locationId, locationGroup);

                return;
            }

            await UpdateStockByLocation(result, locationGroup, _stockPerLocationContainer);
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

        private async Task CreateStockByLocations(Container container, string locationId, StockByStorageLocation storageLocationGroup)
        {
            var storageLocationStock = new StockByStorageLocation
            {
                Id = locationId,
                LocationId = locationId,
                LocationType = storageLocationGroup.LocationType,
                Checkpoints = storageLocationGroup.Checkpoints
            };

            ItemRequestOptions requestOptions = new ItemRequestOptions {EnableContentResponseOnWrite = false};

            var response = await container.CreateItemAsync(storageLocationStock,
                new PartitionKey(locationId), requestOptions);

            Console.WriteLine($"\nRequest Charge for creating storage location: {response.RequestCharge}\n");
        }

        private async Task UpdateStockByLocation(ItemResponse<StockByStorageLocation> result, StockByStorageLocation locationGroup, Container container)
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

        private bool HandleChangesForLocation(StockByStorageLocation locationGroup, List<ArticleCheckpoint> checkpoints, StockByStorageLocation location)
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

            if (locationGroup.Checkpoints.Count <= 0)
                return changesHappened;

            location.Checkpoints.AddRange(locationGroup.Checkpoints);

            return true;
        }
    }
}