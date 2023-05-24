using System.ComponentModel.DataAnnotations;

namespace AirTableDatabase.DBModels
{
    public class ProjectForm
    {
        [Required]
        public string ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Version { get; set; }
        [Required]
        public string ApiKey { get; set; }
        [Required]
        public string ClientPrefixId { get; set; }
        [Required]
        public string Mode { get; set; }
        [Required]
        public string CountryPrefixId { get; set; }
        [Required]
        public string MainDatabaseID { get; set; }
        [Required]
        public string TableSheetsToken { get; set; }
    }
}
