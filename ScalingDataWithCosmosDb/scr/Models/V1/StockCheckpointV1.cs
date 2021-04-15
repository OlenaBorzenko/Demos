using System;
using Newtonsoft.Json;

namespace Models.V1
{
    public class StockCheckpointV1
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string ArticleId { get; set; }

        public string ArticleName { get; set; }

        public int Quantity { get; set; }

        public string LocationId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}