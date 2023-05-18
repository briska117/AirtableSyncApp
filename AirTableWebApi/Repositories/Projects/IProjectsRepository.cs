using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.Projects
{
    public interface IProjectsRepository
    {
        public Task<List<Project>> GetProjects();
        public Task<Project> GetProject(string projectId);
        public Task<bool> AddProject(Project project);
        public Task<bool> DeleteProject(string projectId);
        public Task<bool> UpdateProject(Project project);
        public Task<Project> GetProjectExtend(string projectId);
    }
}
