﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class BasisResponse
    {
        [JsonProperty("bases")]
        public List<Basis> Bases { get; set; }
    }
}
