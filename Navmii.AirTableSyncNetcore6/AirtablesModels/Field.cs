using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class Field
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
