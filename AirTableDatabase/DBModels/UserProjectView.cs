using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableDatabase.DBModels
{
    public class UserProjectView
    {
        public List<UserProject> UserProjects { get; set; } 
        public List<Project> Projects { get; set; }
        public string UserId { get; set; }
    }
}
