using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class CollectionModeRelatedTable
    {
        [Key]
        [Required]
        public string CollectionModeRelatedTableId { get; set; }
        [Required]
        [ForeignKey("CollectionMode")]
        public string CollectionModeId { get; set; }
        [Required]
        [ForeignKey("RelatedTable")]
        public string RelatedTableId { get; set; }

        public bool IsMain { get; set; }    

        public bool IsTeam { get; set; }    
        

        public virtual RelatedTable RelatedTable { get; set; }  

    }
}
