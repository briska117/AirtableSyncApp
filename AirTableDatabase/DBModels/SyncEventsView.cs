using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class SyncEventsView
    {
        public SyncEvent SyncEvent { get; set; }    
        public List<SyncEventHistory> SyncEventHistories { get; set; }  
    }
}
