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
        public DateTime StartAsync { get; set; }

        public DateTime FinishAsync { get; set; }

        public string Comment { get; set; } 

        public StatusSync StatusAsync { get; set; } 

        public virtual SyncEvent SyncEvent { get; set; }  
    }

    public enum StatusSync
    {
        Start,
        Finish,
        InProccess,
        Error,
        Pause
    }
}
