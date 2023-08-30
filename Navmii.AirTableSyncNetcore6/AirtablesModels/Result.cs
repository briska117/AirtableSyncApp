using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class Result
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("options")]
        public Options Options { get; set; }
    }
}
