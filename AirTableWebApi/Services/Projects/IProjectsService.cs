using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.Projects
{
    public interface IProjectsService
    {
        public Task<List<Project>> GetProjects();
        public Task<Project> GetProject(string projectId);
        public Task AddProject(Project project);
        public Task DeleteProject(string projectId);
        public Task UpdateProject(Project project);
        public Task<bool> ProjectExist(string projectId);
        public Task<Project> GetProjectExtend(string projectId);
        public Task<List<WindowsServiceProjectSynchronization>> GetWindowsServiceProjectSynchronization();
    }
}
