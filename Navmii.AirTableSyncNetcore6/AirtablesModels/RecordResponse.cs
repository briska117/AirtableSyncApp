using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class RecordResponse
    {
        [JsonProperty("records")]
        public List<Record> Records { get; set; }
    }
}
