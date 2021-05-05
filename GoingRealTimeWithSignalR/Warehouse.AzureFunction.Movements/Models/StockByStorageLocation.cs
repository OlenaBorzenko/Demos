using System.Collections.Generic;

namespace WarehouseAzureFunctionMovements.Models
{
    public class StockByStorageLocation
    {
        public string id { get; set; }

        public string locationId { get; set; }

        public string locationType { get; set; }

        public List<ArticleCheckpoint> checkpoints { get; set; }
    }
}