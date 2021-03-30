using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.V2
{
    public record StockByLocationV2
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; init; }

        public string LocationId { get; init; }

        public string LocationType { get; init; }

        public List<ArticleCheckpointV2> Checkpoints { get; init; }
    }
}