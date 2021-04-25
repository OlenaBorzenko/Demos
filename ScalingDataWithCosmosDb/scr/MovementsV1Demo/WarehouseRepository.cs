using System;
using System.Collections.Generic;
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

        public WarehouseRepository(string databaseName, string connection)
        {
            CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
            var client = new CosmosClient(connection, options);

            _database = client.CreateDatabaseIfNotExistsAsync(databaseName).Result;

            _database.CreateContainerIfNotExistsAsync(MovementContainerName, "/ArticleId");
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

        public async Task CreateStockCheckpointByLocation()
        {
            Container container = _database.GetContainer(MovementContainerName);

            var fromAggregation = await GroupOutboundMovementsByLocation(container);
            var toAggregation = await GroupInboundMovementsByLocation(container);

            await SaveCheckpointInformation(fromAggregation, toAggregation);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task GetStockByArticles()
        {
            Container container = _database.GetContainer(StockContainerName);

            var result = QueryAggregationByArticle(container);

            while (result.HasMoreResults)
            {
                FeedResponse<StockCheckpointV1> response = await result.ReadNextAsync();

                if (response.Resource.ToList().Count == 0)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockCheckpointV1 stock in response)
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
            Container container = _database.GetContainer(StockContainerName);
            var result = container.GetItemQueryIterator<StockCheckpointV1>();

            while (result.HasMoreResults)
            {
                FeedResponse<StockCheckpointV1> response = await result.ReadNextAsync();

                if (response == null)
                {
                    break;
                }

                Console.WriteLine("Print out all stock checkpoints\n");

                foreach (StockCheckpointV1 stock in response)
                {
                    Helpers.Print(stock);
                }

                Console.WriteLine($"\nGet stock by locations: Request Charge: {response.RequestCharge}\n");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


        #region CreateMovements private methods

        private async Task CreateSingleMovement(Container container)
        {
            var articleMovement = ModelsHelper.CreateArticleMovementModel();

            var partitionKey = new PartitionKey(articleMovement.ArticleId);
            var option = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            var response = await container.CreateItemAsync(articleMovement, partitionKey, option);

            Console.WriteLine($"Request Charge: {response.RequestCharge}");
        }

        #endregion


        #region CreateStockCheckpointByLocation private methods

        private async Task<List<AggregationV1>> GroupOutboundMovementsByLocation(Container container)
        {
            string sqlFromLocation = "SELECT p.FromLocationId AS LocationId, p.ArticleId, p.ArticleName, Count(1) AS Count FROM p GROUP BY p.FromLocationId, p.ArticleId, p.ArticleName";

            return await QueryMovements(sqlFromLocation, container);
        }


        private async Task<List<AggregationV1>> GroupInboundMovementsByLocation(Container container)
        {
            string sqlToLocation = "SELECT p.ToLocationId AS LocationId, p.ArticleId, p.ArticleName, Count(1) AS Count FROM p GROUP BY p.ToLocationId, p.ArticleId, p.ArticleName";

            return await QueryMovements(sqlToLocation, container);
        }

        private async Task<List<AggregationV1>> QueryMovements(string sql, Container container)
        {
            FeedIterator<AggregationV1> result = container.GetItemQueryIterator<AggregationV1>(new QueryDefinition(sql));

            var results = new List<AggregationV1>();

            while (result.HasMoreResults)
            {
                FeedResponse<AggregationV1> response = await result.ReadNextAsync();

                if (response.Resource.ToList().Count == 0)
                {
                    break;
                }

                results.AddRange(response);

                Console.WriteLine($"\nGroup movements by storage location: Request Charge: {response.RequestCharge}\n");
            }

            return results;
        }

        private async Task SaveCheckpointInformation(List<AggregationV1> outboundAggregations, List<AggregationV1> inboundAggregations)
        {
            foreach (var inboundAggregation in inboundAggregations)
            {
                var container = _database.GetContainer(StockContainerName);

                var stockCheckpoint = ModelsHelper.CreateStockCheckpoint(outboundAggregations, inboundAggregation);

                var partitionKey = new PartitionKey(stockCheckpoint.LocationId);
                var response = await container.CreateItemAsync(stockCheckpoint, partitionKey);

                Console.WriteLine($"Create checkpoint: Request Charge: {response.RequestCharge}");
            }
        }

        #endregion


        #region GetStockByArticles private methods

        private FeedIterator<StockCheckpointV1> QueryAggregationByArticle(Container container)
        {
            string sql = "SELECT p.ArticleId, p.ArticleName, SUM(p.Quantity) as Quantity FROM p GROUP BY p.ArticleId, p.ArticleName";

            return container.GetItemQueryIterator<StockCheckpointV1>(new QueryDefinition(sql));
        }

        #endregion
    }
}