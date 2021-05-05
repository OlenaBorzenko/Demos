using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements.Functions
{
    public static class CreateMovements
    {
        [FunctionName("CreateMovements")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "warehouse", collectionName: "movements",
                ConnectionStringSetting = "CosmosDbConnection",
                PartitionKey = "/ArticleId",
                CreateIfNotExists = true)] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            log.LogInformation($"Request body: {data}");

            var tasks = new List<Task>();

            for (var i = 0; i < 2; i++)
            {
                tasks.Add(CreateSingleMovement(documentsOut));
            }

            await Task.WhenAll(tasks);

            return new OkResult();
        }

        private static async Task CreateSingleMovement(IAsyncCollector<dynamic> documentsOut)
        {
            var articleMovement = CreateArticleMovementModel();
            await documentsOut.AddAsync(articleMovement);
        }

        private static ArticleMovement CreateArticleMovementModel()
        {
            // Getting random test data;
            var article = Helpers.GetItemByRandomIndex(TestData.Articles);
            var movement = Helpers.GetItemByRandomIndex(TestData.Movements);

            return new ArticleMovement
            {
                id = Guid.NewGuid().ToString(),
                articleId = article.id,
                articleName = article.name,
                movementType = movement.type,
                fromLocationId = movement.@from,
                toLocationId = movement.to,
                timeStamp = DateTimeOffset.Now
            };
        }
    }
}