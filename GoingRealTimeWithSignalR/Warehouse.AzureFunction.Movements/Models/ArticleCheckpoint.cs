using System;

namespace WarehouseAzureFunctionMovements.Models
{
    public class ArticleCheckpoint
    {
        public string articleId;

        public string articleName;

        public int quantity;

        public DateTimeOffset timeStamp;
    }
}