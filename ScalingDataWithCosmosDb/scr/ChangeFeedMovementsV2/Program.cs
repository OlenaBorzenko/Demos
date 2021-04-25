using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Shared;

namespace ChangeFeedMovementsV2
{
    static class Program
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private static readonly string DatabaseName = Configuration["DatabaseName"];
        private static readonly string Connection = Configuration["CosmosDBConnection"];

        private const string MovementContainerName = "movements";
        private const string StockPerLocationContainerName = "stock-per-location";
        private const string StockPerArticleContainerName = "stock-per-article";

        private static readonly CosmosClient Client = new(Connection, new CosmosClientOptions { AllowBulkExecution = true });
        private static readonly Database Database = Client.GetDatabase(DatabaseName);
        private static readonly InventoryService InventoryService = new(Database);

        public static async Task Main(string[] args)
        {
            ContainerProperties leaseContainerProperties = new ContainerProperties("movements-leases-containerV2", "/id");
            Container leaseContainer = await Database
                .CreateContainerIfNotExistsAsync(leaseContainerProperties, throughput: 400);

            await Database.CreateContainerIfNotExistsAsync(StockPerLocationContainerName, "/LocationId");
            await Database.CreateContainerIfNotExistsAsync(StockPerArticleContainerName, "/ArticleId");

            var processor = Database
                .GetContainer(MovementContainerName)
                .GetChangeFeedProcessorBuilder<ArticleMovement>("MovementProcessor", HandleMovementsChanges)
                .WithInstanceName("ChangeFeedProductCategories")
                .WithLeaseContainer(leaseContainer)
                .Build();

            Console.WriteLine("Starting Change Feed Processor...");
            await processor.StartAsync();
            Console.WriteLine("Change Feed Processor started.");

            Console.WriteLine("Press any key to stop the processor...");
            Console.ReadKey();

            Console.WriteLine("Stopping Change Feed Processor");

            await processor.StopAsync();
        }

        private static async Task HandleMovementsChanges(IReadOnlyCollection<ArticleMovement> changes, CancellationToken cancellationToken)
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