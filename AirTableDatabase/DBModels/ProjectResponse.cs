using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class ProjectResponse
    {
        public Project Project { get; set; }
        public CollectionMode CollectionMode { get; set; }  
        public List<RelatedTable> RelatedTables { get; set; } 
        public List<SyncEvent>  Events { get; set; }   
    }
}
