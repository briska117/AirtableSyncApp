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
    public class RelatedTable
    {
        public string RelatedTableId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string TableId { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool MainList { get; set; }
        public bool TeamList { get; set; }
        [JsonIgnore]
        public virtual ICollection<AirTableField> AirTableFields { get; } = new List<AirTableField>();
        [JsonIgnore]
        public virtual ICollection<CollectionModeRelatedTable> CollectionModeRelatedTables { get; } = new List<CollectionModeRelatedTable>();

    }
}
