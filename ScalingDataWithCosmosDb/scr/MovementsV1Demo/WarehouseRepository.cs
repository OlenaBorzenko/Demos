using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Models.V1;
using Shared;

// - Good practice to provide the PartitionKey, so SDK don't have to extract it from your data;
// - When you need to write a big amount of data into your container enable Bulk by toggling AllowBulkExecution flag;
// - If you don't need the returned resources for write operations you can disable EnableContentResponseOnWrite for those requests;

namespace MovementsV1Demo
{
    public class WarehouseRepository
    {
        private const string MovementContainerName = "movements";
        private const string StockContainerName = "stock-checkpoints";
        private readonly Database _database;

        public WarehouseRepository(string databaseName, string endpoint, string key)
        {
            CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
            var client = new CosmosClient(endpoint, key, options);

            _database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

            _database.CreateContainerIfNotExistsAsync(MovementContainerName, "/ArticleId", 2000);
            _database.CreateContainerIfNotExistsAsync(StockContainerName, "/LocationId");
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

        public async Task CreateAggregationByLocation()
        {
            Container container = _database.GetContainer(MovementContainerName);

            var fromAggregation = await GetOutboundAggregationByLocation(container);
            var toAggregation = await GetInboundAggregationByLocation(container);

            await SaveCheckpointInformation(fromAggregation, toAggregation);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task GetAggregationByArticle()
        {
            Container container = _database.GetContainer(StockContainerName);

            var result = QueryAggregationByArticle(container);

            while (result.HasMoreResults)
            {
                FeedResponse<StockCheckpointV1> response = await Helpers
                    .MakeRequestWithStopWatch(() => result.ReadNextAsync(), "Get aggregation by article");

                if (response == null)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockCheckpointV1 stock in response)
                {
                    Helpers.Print(stock);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task QueryAggregateByLocation()
        {
            Container container = _database.GetContainer(StockContainerName);

            var result = container.GetItemQueryIterator<StockCheckpointV1>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockCheckpointV1> response = await Helpers
                    .MakeRequestWithStopWatch(() => result.ReadNextAsync(), "Group items by location");

                if (response == null)
                {
                    break;
                }


                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockCheckpointV1 stock in response)
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
           // Creation article movement model based on test data from a previous step;
            var articleMovement = CreateArticleMovementModel();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var partitionKey = new PartitionKey(articleMovement.ArticleId);

            // Saving document in CosmosDB;
            var response = await container.CreateItemAsync(articleMovement, partitionKey);

            stopWatch.Stop();

            Console.WriteLine($"Request Charge: {response.RequestCharge}. Time spend in ms: {stopWatch.Elapsed.Milliseconds}");
        }

        private ArticleMovement CreateArticleMovementModel() {
            // Getting random test data;
            var article = Helpers.GetItemByRandomIndex(TestData.Articles);
            var movement = Helpers.GetItemByRandomIndex(TestData.Movements);

            return new ArticleMovement
            {
                Id = Guid.NewGuid().ToString(),
                ArticleId = article.Id,
                ArticleName = article.Name,
                MovementType = movement.Type,
                FromLocationId = movement.From,
                ToLocationId = movement.To,
                TimeStamp = DateTimeOffset.Now
            };
        }

        #endregion


        #region CreateAggregationByLocation private methods

        private async Task<List<AggregationV1>> GetOutboundAggregationByLocation(Container container)
        {
            string sqlFromLocation = "SELECT p.FromLocationId AS LocationId, p.ArticleId, p.ArticleName, Count(1) AS Count FROM p GROUP BY p.FromLocationId, p.ArticleId, p.ArticleName";

            return await AggregateMovements(sqlFromLocation, container);
        }


        private async Task<List<AggregationV1>> GetInboundAggregationByLocation(Container container)
        {
            string sqlToLocation = "SELECT p.ToLocationId AS LocationId, p.ArticleId, p.ArticleName, Count(1) AS Count FROM p GROUP BY p.ToLocationId, p.ArticleId, p.ArticleName";

            return await AggregateMovements(sqlToLocation, container);
        }

        private async Task<List<AggregationV1>> AggregateMovements(string sql, Container container)
        {
            FeedIterator<AggregationV1> result = container.GetItemQueryIterator<AggregationV1>(new QueryDefinition(sql));

            var results = new List<AggregationV1>();

            while (result.HasMoreResults)
            {
                FeedResponse<AggregationV1> response = await Helpers
                    .MakeRequestWithStopWatch(() => result.ReadNextAsync(), "Aggregate quantity of articles by location");

                if (response == null)
                {
                    break;
                }

                results.AddRange(response);
            }

            return results;
        }

        private async Task SaveCheckpointInformation(List<AggregationV1> fromAggregation, List<AggregationV1> toAggregation)
        {
            foreach (var toItem in toAggregation)
            {
                var fromItem = fromAggregation
                    .FirstOrDefault(x => x.LocationId == toItem.LocationId && x.ArticleId == toItem.ArticleId);

                var container = _database.GetContainer(StockContainerName);

                var stockCheckpoint = new StockCheckpointV1
                {
                    Id = Guid.NewGuid().ToString(),
                    ArticleId = toItem.ArticleId,
                    ArticleName = toItem.ArticleName,
                    Quantity = fromItem == null ? toItem.Count : toItem.Count - fromItem.Count,
                    LocationId = toItem.LocationId,
                    TimeStamp = DateTimeOffset.Now
                };

                var partitionKey = new PartitionKey(stockCheckpoint.LocationId);

                await Helpers.MakeRequestWithStopWatch(() => container.CreateItemAsync(stockCheckpoint, partitionKey), "Create checkpoint");
            }
        }

        #endregion


        #region QueryAggregateByArticle private methods

        private FeedIterator<StockCheckpointV1> QueryAggregationByArticle(Container container)
        {
            string sql = "SELECT p.ArticleId, p.ArticleName, SUM(p.Quantity) as Quantity FROM p GROUP BY p.ArticleId, p.ArticleName";

            return container.GetItemQueryIterator<StockCheckpointV1>(new QueryDefinition(sql));
        }

        #endregion
    }
}