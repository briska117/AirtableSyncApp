using Newtonsoft.Json;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class Table
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("primaryFieldId")]
        public string PrimaryFieldId { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty("views")]
        public List<View> Views { get; set; }
    }
}
