using System;
using Newtonsoft.Json;

namespace WarehouseAzureFunctionMovements.Models
{
    public class ArticleMovement
    {
        public string id { get; set; }

        public string articleId { get; set; }

        public string articleName { get; set; }

        public string movementType { get; set; }

        public string fromLocationId { get; set; }

        public string toLocationId { get; set; }

        public DateTimeOffset timeStamp { get; set; }
    }
}