using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Models.V2;
using Shared;

namespace MovementsV2Demo
{
    public class WarehouseRepository
    {
        private const string MovementContainerName = "movements";
        private const string StockPerLocationContainerName = "stock-per-location";
        private const string StockPerArticleContainerName = "stock-per-article";
        private readonly Database _database;

        public WarehouseRepository(string databaseName, string endpoint, string key)
        {
            CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
            var client = new CosmosClient(endpoint, key, options);

            _database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

            client.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(MovementContainerName, "/ArticleId");
            client.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/LocationId");
            client.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(StockPerArticleContainerName, "/ArticleId");
        }

        public async Task CreateMovements(int amount)
        {
            Container container = _database.GetContainer(MovementContainerName);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < amount; i++)
            {
                tasks.Add(CreateSingleMovement(container));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task GetAggregationByArticle()
        {
            Container container = _database.GetContainer(StockPerArticleContainerName);

            var result = container.GetItemQueryIterator<StockByArticleV2>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockByArticleV2> response = await Helpers
                    .MakeRequestWithStopWatch(() => result.ReadNextAsync(), "Get stock by article");

                if (response == null)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockByArticleV2 stock in response)
                {
                    Helpers.Print(stock);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task QueryAggregateByLocation()
        {
            Container container = _database.GetContainer(StockPerLocationContainerName);

            var result = container.GetItemQueryIterator<StockByLocationV2>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockByLocationV2> response = await Helpers
                    .MakeRequestWithStopWatch(() => result.ReadNextAsync(), "Get stock by article");

                if (response == null)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockByLocationV2 stock in response)
                {
                    Helpers.Print(stock);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


        #region CreateMovements private methods

        private async Task CreateSingleMovement(Container container)
        {
            // Getting random test data;
            var article = Helpers.GetItemByRandomIndex(TestData.Articles);
            var movement = Helpers.GetItemByRandomIndex(TestData.Movements);

            // Creation article movement model based on test data from a previous step;
            var articleMovement = CreateArticleMovementModel(article, movement);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var partitionKey = new PartitionKey(articleMovement.ArticleId);
            // Saving document in CosmosDB;
            var response = await container.CreateItemAsync(articleMovement, partitionKey);

            stopWatch.Stop();

            Console.WriteLine($"Request Charge: {response.RequestCharge}. Time spend in ms: {stopWatch.Elapsed.Milliseconds}");
        }

        private ArticleMovement CreateArticleMovementModel(ArticleItem article, MovementItem movement) =>
            new()
            {
                Id = Guid.NewGuid().ToString(),
                ArticleId = article.Id,
                ArticleName = article.Name,
                MovementType = movement.Type,
                FromLocationId = movement.From,
                ToLocationId = movement.To,
                TimeStamp = DateTimeOffset.Now
            };

        #endregion
    }
}