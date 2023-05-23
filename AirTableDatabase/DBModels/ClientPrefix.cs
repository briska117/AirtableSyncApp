using System.ComponentModel.DataAnnotations;

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
