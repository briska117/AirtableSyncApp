using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class AirTableField
    {
       
        public string AirTableFieldId { get; set; }
        [ForeignKey("RelatedTable")]
        public string RelatedTableId { get; set; }
        public string FieldName { get; set; }
        public int Order { get; set; }
        [JsonIgnore]
        public virtual RelatedTable RelatedTable { get; set; }
    }
}
