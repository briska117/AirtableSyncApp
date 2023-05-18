using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class CollectionRelatedRequest
    {
        public string CollectionModeId { get; set; }    
        public List<string> RelatedTablesIds { get; set; }  
    }
}
