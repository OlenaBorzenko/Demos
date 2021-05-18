using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace WarehouseAzureFunctionMovements.Functions
{
    public static class StockChangeTrigger
    {
        [FunctionName("StockChangeTrigger")]
        public static async Task RunAsync(
            [CosmosDBTrigger(databaseName: "warehouse", collectionName: "stock-per-location",
                ConnectionStringSetting = "CosmosDBConnection", LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> input, ILogger log,
            [SignalR(HubName = "warehouse")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Sending messages to signalr");

                var message = new SignalRMessage
                {
                    Target = "stockPerLocationChanged",
                    Arguments = new object[] {input}
                };

                await signalRMessages.AddAsync(message);
            }
        }
    }
}