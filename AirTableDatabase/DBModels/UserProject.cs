using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirTableDatabase.DBModels
{
    public class UserProject
    {
        [Key]
        [Required]
        public string UserProjectId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("Project")]
        public string ProjectId { get;set; }
        public virtual Project ProjectAsync { get; set; }
    }
}
