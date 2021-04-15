using System;
using Newtonsoft.Json;

namespace Shared
{
    public class ArticleMovement
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string ArticleId { get; set; }

        public string ArticleName { get; set; }

        public string MovementType { get; set; }

        public string FromLocationId { get; set; }

        public string ToLocationId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}