using System;
using System.Collections.Generic;
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

        public WarehouseRepository(string databaseName, string connection)
        {
            CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
            var client = new CosmosClient(connection, options);

            _database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

            client.GetDatabase(databaseName).CreateContainerIfNotExistsAsync(MovementContainerName, "/ArticleId");
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

        public async Task GetStockByArticles()
        {
            Container container = _database.GetContainer(StockPerArticleContainerName);

            var result = container.GetItemQueryIterator<StockByArticleV2>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockByArticleV2> response = await result.ReadNextAsync();

                if (response == null)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockByArticleV2 stock in response)
                {
                    Helpers.Print(stock);
                }

                Console.WriteLine($"\nGet stock by articles: Request Charge: {response.RequestCharge}\n");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task GetStockByLocations()
        {
            Container container = _database.GetContainer(StockPerLocationContainerName);
            var result = container.GetItemQueryIterator<StockByLocationV2>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockByLocationV2> response = await result.ReadNextAsync();

                if (response == null)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockByLocationV2 stock in response)
                {
                    Helpers.Print(stock);
                }

                Console.WriteLine($"\nGet stock by location: Request Charge: {response.RequestCharge}\n");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        #region CreateMovements private methods

        private async Task CreateSingleMovement(Container container)
        {
            var articleMovement = ModelsHelper.CreateArticleMovementModel();

            var partitionKey = new PartitionKey(articleMovement.ArticleId);
            var response = await container.CreateItemAsync(articleMovement, partitionKey);

            Console.WriteLine($"Request Charge: {response.RequestCharge}.");
        }
        #endregion
    }
}