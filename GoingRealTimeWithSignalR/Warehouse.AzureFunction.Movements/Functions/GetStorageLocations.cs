using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace WarehouseAzureFunctionMovements.Functions
{
    public static class GetStorageLocations
    {
        private static readonly string Connection = Environment.GetEnvironmentVariable("CosmosDBConnection");

        private static readonly CosmosClient Client = new CosmosClient(Connection);
        private static readonly Database Database = Client.GetDatabase("warehouse");

        private static readonly InventoryService InventoryService = new InventoryService(Database);

        [FunctionName("GetStorageLocations")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request. Get storage locations;");

            return new OkObjectResult(await InventoryService.GetInventoryOnStorageLocations());
        }
    }
}