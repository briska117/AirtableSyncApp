using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class ClientPrefix
    {
        [Key]
        [Required]
        public string ClientPrefixId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
