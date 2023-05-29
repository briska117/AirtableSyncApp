using System.ComponentModel.DataAnnotations;

namespace AirTableDatabase.DBModels
{
    public class UserProjectRequest
    {
        public string UserProjectId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ProjectId { get; set; }
    }
}
