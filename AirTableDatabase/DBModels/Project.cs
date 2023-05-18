using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class Project
    {
        [Key]
        [Required]
        public string ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Version { get; set; }
        [Required]
        public string ApiKey { get; set; }
        [Required]
        [ForeignKey("ClientPrefix")]
        public string ClientPrefixId { get; set; }
        [Required]
        [ForeignKey("CollectionMode")]
        public string Mode { get; set; }
        [Required]
        [ForeignKey("CountryPrefix")]
        public string CountryPrefixId { get; set; }
        [Required]
        public string MainDatabaseID { get; set; }
        [Required]
        public string TableSheetsToken { get; set; }

        public virtual ClientPrefix ClientPrefix { get; set; }

        public virtual CollectionMode CollectionMode { get; set; }

        public virtual CountryPrefix CountryPrefix { get; set; }



    }
}
