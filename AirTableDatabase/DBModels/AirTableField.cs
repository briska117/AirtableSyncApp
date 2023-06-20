using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class AirTableField
    {
        [Key]
        public string AirTableFieldId { get; set; }
        [Required]
        public string RelatedTableId { get; set; }
        [Required]
        public string FieldName { get; set; }
    }
}
