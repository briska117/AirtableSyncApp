using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class TablesResponse
    {
        [JsonProperty("tables")]
        public List<Table> Tables { get; set; }
    }
}
