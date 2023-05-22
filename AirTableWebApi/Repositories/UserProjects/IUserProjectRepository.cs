using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.UserProjects
{
    public interface IUserProjectRepository
    {
        public Task<UserProject> GetUserProject(string id);
        public Task<List<UserProject>> GetUserProjects();
        public Task<UserProject> AddUserProject(UserProject userProject);
        public Task<UserProject> UpdateUserProject(UserProject userProject);
        public Task<bool> RemoveUserProject(string id);
    }
}
