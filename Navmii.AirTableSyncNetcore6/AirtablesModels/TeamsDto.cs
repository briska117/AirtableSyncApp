using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class TeamsDto
    {
        public string DataBaseId { get; set; }
        public string Name { get; set; } 
        public List<TableDto> TeamTables { get; set; }
    }
}
