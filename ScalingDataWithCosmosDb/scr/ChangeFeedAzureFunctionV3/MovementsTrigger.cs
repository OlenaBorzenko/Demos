using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using Database = Microsoft.Azure.Cosmos.Database;

namespace ChangeFeedAzureFunctionV3
{
    public static class MovementsTrigger
    {
        #region PrivateFields

        private static readonly string DatabaseName = Environment.GetEnvironmentVariable("CosmosDBDatabase");
        private static readonly string Connection = Environment.GetEnvironmentVariable("CosmosDBConnection");

        private static readonly CosmosClient Client = new CosmosClient(Connection);
        private static readonly Database Database = Client.GetDatabase(DatabaseName);

        private static readonly InventoryService InventoryService = new InventoryService(Database);

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

            var (movementsByLocation, movementsByArticle) = StockHelper.GroupInboundOutboundStock(changes);

            var tasks = movementsByLocation
                .Select(locationGroup => InventoryService.CalculateInventoryOnStorageLocations(locationGroup)).ToList();

            tasks.AddRange(movementsByArticle
                .Select(articleGroup => InventoryService.CalculateInventoryForArticles(articleGroup)));

            await Task.WhenAll(tasks);
        }
    }
}