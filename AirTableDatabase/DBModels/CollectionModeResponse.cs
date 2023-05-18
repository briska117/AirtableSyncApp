using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class CollectionModeResponse
    { 
        public CollectionMode CollectionMode { get; set; }
        public List<RelatedTable> relatedTables { get; set; }
    }
}
