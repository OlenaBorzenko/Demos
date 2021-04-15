using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.V2
{
    public class StockByLocationV2
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string LocationId { get; set; }

        public string LocationType { get; set; }

        public List<ArticleCheckpointV2> Checkpoints { get; set; }
    }
}