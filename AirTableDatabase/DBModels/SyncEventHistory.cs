using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public  class SyncEventHistory
    {
        [Required]
        [Key]
        public string SyncEventHistoryId { get; set; }
        [ForeignKey("SyncEvent")]
        [Required]
        public string SyncEventId { get; set; }
        public DateTime StartSync { get; set; }

        public DateTime FinishSync { get; set; }

        public string Comment { get; set; } 

        public StatusSync StatusSync { get; set; } 

        public virtual SyncEvent SyncEvent { get; set; }  
    }

    public enum StatusSync
    {
        Start,
        Complete,
        Finish,
        InProccess,
        Error,
        Pause
    }
}
