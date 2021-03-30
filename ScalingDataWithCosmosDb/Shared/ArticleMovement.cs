using System;
using Newtonsoft.Json;

namespace Shared
{
    public record ArticleMovement
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; init; }

        public string ArticleId { get; init; }

        public string ArticleName { get; init; }

        public string MovementType { get; init; }

        public string FromLocationId { get; init; }

        public string ToLocationId { get; init; }

        public DateTimeOffset TimeStamp { get; init; }
    }
}