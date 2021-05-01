using System;

namespace WarehouseAzureFunctionMovements.Models
{
    public class ArticleCheckpoint
    {
        public string ArticleId;

        public string ArticleName;

        public int Quantity;

        public DateTimeOffset TimeStamp;
    }
}