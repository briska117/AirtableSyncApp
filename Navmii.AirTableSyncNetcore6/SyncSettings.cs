using AirTableDatabase.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirTableSyncNetcore6
{
    public class SyncSettings
    {
        public string BackupPath { get; set; }  

        public bool ManualSync { get; set; }
        public bool SyncTSheets { get; set; }

        public string ApiKey { get; set; }

        public TimeSpan SyncTime1 { get; set; }
        public TimeSpan SyncTime2 { get; set; }
        public TimeSpan SyncTime3 { get; set; }
        public string ProjectPrefix { get; set; }

        public string MainDatabaseID { get; set; }

        public string TSheetsApiToken { get; set; }

        public string CollectionModeId { get; set; }

        public string[] TableListCentral { get; set; }
        public string[] TableListLocal { get; set; }

        public List<RelatedTable> RelatedTables { get; set; }
    }
}
