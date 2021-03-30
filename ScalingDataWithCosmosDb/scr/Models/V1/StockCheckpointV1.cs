using System;
using Newtonsoft.Json;

namespace Models.V1
{
    public record StockCheckpointV1
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; init; }

        public string ArticleId { get; init; }

        public string ArticleName { get; init; }

        public int Quantity { get; init; }

        public string LocationId { get; init; }

        public DateTimeOffset TimeStamp { get; init; }
    }
}