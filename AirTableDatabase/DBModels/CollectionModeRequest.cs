using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class CollectionModeRequest
    {
        public CollectionMode CollectionMode { get; set; }  

        public List<RelatedTableDto> RelatedTables { get; set; }

        public bool TablesHaveChange { get; set; }
    }
}
