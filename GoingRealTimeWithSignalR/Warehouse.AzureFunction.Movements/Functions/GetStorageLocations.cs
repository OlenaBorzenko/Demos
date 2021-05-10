using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements.Functions
{
    public static class GetStorageLocations
    {
        [FunctionName("GetStorageLocations")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "warehouse",
                collectionName: "stock-per-location",
                ConnectionStringSetting = "CosmosDBConnection")]
            IEnumerable<StockByStorageLocation> storageLocations, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request. Get storage locations;");

            return new OkObjectResult(storageLocations);
        }
    }
}