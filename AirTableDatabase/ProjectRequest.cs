using AirTableDatabase.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase
{
    public class ProjectRequest
    {
        public Project Project { get; set; }
        public List<SyncEvent> Events { get; set; }
    }
}
