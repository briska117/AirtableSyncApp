using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.UserProjects
{
    public class UserProjectRepository : IUserProjectRepository
    {
        private readonly ApplicationDBContext applicationDB;

        public UserProjectRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }
        public async Task<UserProject> AddUserProject(UserProject userProject)
        {
            try
            {
                userProject.UserProjectId = Guid.NewGuid().ToString();
                await this.applicationDB.UserProjects.AddAsync(userProject);
                await this.applicationDB.SaveChangesAsync();
                return userProject;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Add User Project : {ex.Message}");
            }
        }

        public async Task<List<UserProject>> GetProjectsByUser(string userId)
        {
            try
            {
                List<UserProject> userProjects = await this.applicationDB.UserProjects.Where(cp => cp.UserId == userId)
                    .Include(cp=> cp.Project.ClientPrefix)
                    .Include(cp => cp.Project.CollectionMode)
                    .Include(cp => cp.Project.CountryPrefix)
                    .ToListAsync();
                return userProjects;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in get Projects with user Id {userId} : {ex.Message}");
            }
        }

        public async Task<UserProject> GetUserProject(string id)
        {
            try
            {
                UserProject userProject = this.applicationDB.UserProjects.FirstOrDefault(cp => cp.UserProjectId == id);
                return userProject;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in get User Project with Id {id} : {ex.Message}");
            }
        }

        public async Task<List<UserProject>> GetUserProjects()
        {
            try
            {
                List<UserProject> userProjects = await this.applicationDB.UserProjects.Include(p => p.Project).ToListAsync();
                return userProjects;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in get List of User Projects : {ex.Message}");
            }
        }

        public async Task<bool> RemoveUserProject(string id)
        {
            try
            {
                UserProject userProject = await GetUserProject(id);
                this.applicationDB.UserProjects.Remove(userProject);
                await this.applicationDB.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in remove User Project with Id {id} : {ex.Message}");
            }
        }

        public async Task<UserProject> UpdateUserProject(UserProject userProject)
        {
            try
            {
                UserProject userProjectDB = await GetUserProject(userProject.UserProjectId);
                userProjectDB.UserId = userProject.UserId;
                userProjectDB.ProjectId = userProject.ProjectId;
                this.applicationDB.UserProjects.Entry(userProjectDB);
                await this.applicationDB.SaveChangesAsync();
                return userProjectDB;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in update User Project with Id {userProject.UserProjectId} : {ex.Message}");
            }
        }
    }
}
