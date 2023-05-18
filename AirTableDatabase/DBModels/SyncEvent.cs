using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class SyncEvent
    {
      
        [Key]
        public string SyncEventId { get; set; }
        [Required]
        public string EventName { get; set; }

        [ForeignKey("Project")]
        public string ProjectId { get; set; }

        [Required]
        public DateTime SyncTime { get; set; }
    }
}
