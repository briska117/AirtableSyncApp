using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.Projects;

namespace AirTableWebApi.Services.Projects
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository projects;

        public ProjectsService(IProjectsRepository projects)
        {
            this.projects = projects;
        }
        public async Task AddProject(Project project)
        {
            var result = await this.projects.AddProject(project);
            if (!result)
            {
                throw new Exception("Error in Crate Project");
            }

        }

        public async Task DeleteProject(string projectId)
        {
            var project = this.GetProject(projectId);
            if(project == null)
            {
                throw new Exception($"Not found project with CountryPrefixId {projectId}");    
            }
            var result = await this.projects.DeleteProject(projectId);
            if (!result)
            {
                throw new Exception($"Error in Delete Project with CountryPrefixId {projectId}");
            }
            
        }

        public Task<Project> GetProject(string projectId)
        {
            var project = this.projects.GetProject(projectId);
            if (project == null)
            {
                throw new Exception($"Not found project with CountryPrefixId {projectId}");
            }
            return project; 
        }

        public async Task<List<Project>> GetProjects()
        {
            return await this.projects.GetProjects();     
        }

        public async Task UpdateProject(Project project)
        {
            
            var result = await this.projects.UpdateProject(project);
            if (!result)
            {
                throw new Exception($"Error in Update Project with CountryPrefixId {project.ProjectId}");
            }
             
        }

        public async Task<bool> ProjectExist(string projectId)
        {
            var projectExist = await this.projects.GetProject(projectId);
            if (projectExist == null)
            {
                return false;
            }
            return true;    
        }

        public Task<Project> GetProjectExtend(string projectId)
        {
            return this.projects.GetProjectExtend(projectId);   
        }
    }
}
