using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.UserProjects;
using AirTableWebApi.Services.Projects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using AutoMapper;
using AirTableDatabase;
using Intuit.TSheets.Model;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProjectsController : ControllerBase
    {
        private readonly IUserProjectService userProjectService;
        private readonly IProjectsService projectsService;
        private readonly IMapper mapper;

        public UserProjectsController(IUserProjectService userProjectService, IMapper mapper, IProjectsService projectsService)
        {
            this.userProjectService = userProjectService;
            this.mapper = mapper;
            this.projectsService = projectsService;
        }

        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserProjects()
        {
            var userProjects = await userProjectService.GetUserProjects();
            return Ok(userProjects);
        }

        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserProject([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var exist = await userProjectService.ExistUserProject(id);
            if (!exist)
            {
                return NotFound();
            }
            var userProjectId = await userProjectService.GetUserProject(id);
            return Ok(userProjectId);
        }

        [HttpGet("GetProjectsFromUser")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetProjectsFromUser(string userId)
        {
            var projectsFromUser = await this.userProjectService.GetProjectsByUser(userId);
            if(projectsFromUser == null)
            {
                return NotFound();
            }

            return Ok(projectsFromUser);
        }

        [HttpGet("GetProjectsFromUserView/{userId}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetProjectsFromUserView(string userId)
        {
            var projectsFromUser = await this.userProjectService.GetProjectsByUser(userId);
            if (projectsFromUser == null)
            {
                return NotFound();
            }
            var projects =await this.projectsService.GetProjects();

            var view = new UserProjectView {
                UserProjects = projectsFromUser,
                Projects = projects,
                UserId = userId
            };
            return Ok(view);
        }

        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddUserProject([FromBody] UserProjectRequest userProject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            UserProject user = new UserProject
            {
                ProjectId = userProject.ProjectId,
                UserId = userProject.UserId
            };
            await userProjectService.AddUserProject(user);
            return Ok(user);
        }

        [HttpPost("AddOrUpdateUserProjects")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddOrUpdateUserProjects([FromBody] List<UserProjectRequest> userProjects)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Add the news
            //Check if exist
            List<string> projectIds = userProjects.Select(up => up.ProjectId).ToList();
            foreach(string projectId in projectIds)
            {
                if (! await this.projectsService.ProjectExist(projectId))
                {
                    return NotFound($"Not found project with id: {projectId}");
                }
            }//user projects from db
            string userId = userProjects.Select(u => u.UserId).FirstOrDefault();
            List<UserProject> usersProjectsDB = await this.userProjectService.GetProjectsByUser(userId); 
            //The news
            List<UserProjectRequest> addList = userProjects.Where(up => up.UserProjectId=="").ToList();
            foreach(UserProjectRequest userProject in addList)
            {
                if(! await userProjectService.ExistUserProject(userProject.UserProjectId) &&
                    usersProjectsDB.Where(u=>u.ProjectId == userProject.ProjectId).Count() == 0)
                {
                    UserProject project = this.mapper.Map<UserProject>(userProject);
                    await this.userProjectService.AddUserProject(project);
                    userProjects.Remove(userProject);
                }
            }
            List<UserProject> usersRemove = usersProjectsDB.Where(u=>!projectIds.Contains(u.ProjectId)).ToList();


            //Delete the missing
            foreach (UserProject userProject in usersRemove)
            {
                await this.userProjectService.DeleteUserProject(userProject.UserProjectId);
            }
            return Ok(await this.userProjectService.GetProjectsByUser(userId));
        }

        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateUserProject([FromBody] UserProjectRequest userProject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var exist = await userProjectService.ExistUserProject(userProject.UserProjectId);
            if (!exist)
            {
                return NotFound();
            }
            UserProject user = new UserProject
            {
                UserProjectId = userProject.UserProjectId,
                ProjectId = userProject.ProjectId,
                UserId = userProject.UserId
            };
            await userProjectService.UpdateUserProject(user);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteUserProject([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var exist = await userProjectService.ExistUserProject(id);
            if (!exist)
            {
                return NotFound();
            }
            var userProjectId = await userProjectService.DeleteUserProject(id);
            return Ok(userProjectId);
        }
    }
}
