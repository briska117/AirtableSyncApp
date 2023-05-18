using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.Projects
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ApplicationDBContext dBContext;

        public ProjectsRepository(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<bool> AddProject(Project project)
        {
            try
            {
                var projectId = Guid.NewGuid();
                project.ProjectId= projectId.ToString();
                await this.dBContext.Projects.AddAsync(project);
                this.dBContext.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
            
        }

        public async Task<bool> DeleteProject(string projectId)
        {
            try
            {
                Project project = this.dBContext.Projects.FirstOrDefault(p => p.ProjectId.Equals(projectId));
                this.dBContext.Projects.Remove(project);
                this.dBContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public async Task<Project> GetProject(string projectId)
        {
            try
            {
                Project project =  this.dBContext.Projects.FirstOrDefault(p => p.ProjectId.Equals(projectId));
                return project;
            }
            catch (Exception)
            {

                return null;   
            }
            
        }

        public async Task<Project> GetProjectExtend(string projectId)
        {
            try
            {
                Project project =await this.dBContext.Projects.Where(p => p.ProjectId.Equals(projectId))
                    .Include(p=>p.ClientPrefix)
                    .Include(p=>p.CollectionMode)
                    .Include(p=>p.CountryPrefix)
                    .FirstOrDefaultAsync();
                return project;
            }
            catch (Exception)
            {

                return null;
            }

        }

        public async Task<List<Project>> GetProjects()
        {
            try
            {
                var projects = this.dBContext.Projects.ToList();
                return projects;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<bool> UpdateProject(Project proyect)
        {
            try
            {
                Project dbProyect =await this.GetProject(proyect.ProjectId);
                dbProyect.Name = proyect.Name;
                dbProyect.ApiKey = proyect.ApiKey;
                dbProyect.CountryPrefixId = proyect.CountryPrefixId;
                dbProyect.ClientPrefixId = proyect.ClientPrefixId;
                dbProyect.MainDatabaseID = proyect.MainDatabaseID;
                dbProyect.Mode = proyect.Mode;
                dbProyect.TableSheetsToken = proyect.TableSheetsToken;
                dbProyect.Version = proyect.Version;
                this.dBContext.Projects.Update(dbProyect);
                this.dBContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {

                return false;
            }
            
        }
    }
}
