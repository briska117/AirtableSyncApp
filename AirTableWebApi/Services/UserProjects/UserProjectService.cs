using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.UserProjects;

namespace AirTableWebApi.Services.UserProjects
{
    public class UserProjectService : IUserProjectService
    {
        private readonly IUserProjectRepository userProjectRepository;

        public UserProjectService(IUserProjectRepository userProjectRepository)
        {
            this.userProjectRepository = userProjectRepository;
        }

        public async Task<UserProject> AddUserProject(UserProject userProject)
        {
            await this.userProjectRepository.AddUserProject(userProject);
            return userProject;
        }

        public async Task<bool> DeleteUserProject(string id)
        {
            bool success = await this.userProjectRepository.RemoveUserProject(id);
            return success;
        }

        public async Task<bool> ExistUserProject(string id)
        {
            var exist = await this.userProjectRepository.GetUserProject(id);
            if (exist == null)
            {
                return false;
            }
            return true;
        }

        public Task<List<UserProject>> GetProjectsByUser(string userId)
        {
            return this.userProjectRepository.GetProjectsByUser(userId);
        }

        public async Task<UserProject> GetUserProject(string id)
        {
            return await this.userProjectRepository.GetUserProject(id);
        }

        public Task<List<UserProject>> GetUserProjects()
        {
            return this.userProjectRepository.GetUserProjects();
        }
        public async Task<UserProject> UpdateUserProject(UserProject userProject)
        {
            return await this.userProjectRepository.UpdateUserProject(userProject);
        }
    }
}
