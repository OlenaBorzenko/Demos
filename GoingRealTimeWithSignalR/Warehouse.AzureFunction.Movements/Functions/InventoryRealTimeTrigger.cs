using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WarehouseAzureFunctionMovements.Models;

namespace WarehouseAzureFunctionMovements.Functions
{
    public static class InventoryRealTimeTrigger
    {
        [FunctionName("InventoryRealTimeTrigger")]
        public static async Task RunAsync([CosmosDBTrigger(
                databaseName: "warehouse",
                collectionName: "stock-per-location",
                ConnectionStringSetting = "CosmosDBConnection",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input, ILogger log,
            [SignalR(HubName = "warehouse")]
            IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);

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