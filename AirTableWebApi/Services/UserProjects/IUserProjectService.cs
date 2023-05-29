using AirTableDatabase.DBModels;
namespace AirTableWebApi.Services.UserProjects
{
    public interface IUserProjectService
    {
        public Task<bool> ExistUserProject(string id);
        public Task<UserProject> GetUserProject(string id);
        public Task<List<UserProject>> GetUserProjects();
        public Task<List<UserProject>> GetProjectsByUser(string userId);
        public Task<UserProject> AddUserProject(UserProject userProject);
        public Task<UserProject> UpdateUserProject(UserProject userProject);
        public Task<bool> DeleteUserProject(string id);
    }
}
