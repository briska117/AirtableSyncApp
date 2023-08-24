using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class RelatedTableDto
    {
        public string TableId { get; set; }
        public bool IsMain { get; set; }    
        public bool IsTeam { get; set; }  
        
    }
}
