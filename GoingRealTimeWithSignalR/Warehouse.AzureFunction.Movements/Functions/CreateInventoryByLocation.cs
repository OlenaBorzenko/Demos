using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WarehouseAzureFunctionMovements.Models;
using Database = Microsoft.Azure.Cosmos.Database;

namespace WarehouseAzureFunctionMovements.Functions
{
    public static class CreateInventoryByLocation
    {
        private static readonly string Connection = Environment.GetEnvironmentVariable("CosmosDBConnection");

        private static readonly CosmosClient Client = new CosmosClient(Connection);
        private static readonly Database Database = Client.GetDatabase("warehouse");

        private static readonly InventoryService InventoryService = new InventoryService(Database);

        [FunctionName("CreateInventoryByLocation")]
        public static async Task RunAsync([CosmosDBTrigger(
                databaseName: "warehouse",
                collectionName: "movements",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "leases",
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

            var movementsByLocation = StockHelper.GroupInboundOutboundStock(changes);

            var tasks = movementsByLocation
                .Select(InventoryService.CalculateInventoryOnStorageLocations)
                .ToList();

            await Task.WhenAll(tasks);
        }
    }
}