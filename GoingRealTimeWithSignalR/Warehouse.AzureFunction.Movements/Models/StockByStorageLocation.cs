using System.Collections.Generic;
using Newtonsoft.Json;

namespace WarehouseAzureFunctionMovements.Models
{
    public class StockByStorageLocation
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string LocationId { get; set; }

        public string LocationType { get; set; }

        public List<ArticleCheckpoint> Checkpoints { get; set; }
    }
}