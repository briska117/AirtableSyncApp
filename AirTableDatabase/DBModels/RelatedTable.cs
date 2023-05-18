using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class RelatedTable
    {
        [Key]
        [Required]
        public string RelatedTableId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string TableId { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
