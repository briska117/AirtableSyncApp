using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6.AirtablesModels
{
    public class AirTablesInfo
    {
        public Basis CentralDataBase { get; set; }
        public List<TableDto> CentralTables { get; set; }
        public Basis LocalDataBase { get; set; }
        public List<TableDto> LocalTables { get; set; }
        public List<TeamsDto> Teams { get; set; }   
    }
}
