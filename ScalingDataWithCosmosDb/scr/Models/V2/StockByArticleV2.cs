using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.V2
{
    public class StockByArticleV2
    {
        [JsonProperty(PropertyName = "id")]
        public string Id;

        public string ArticleId;

        public string ArticleName;

        public List<LocationCheckpointV2> Checkpoints;
    }
}