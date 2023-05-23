using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.UserProjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProjectsController : ControllerBase
    {
        private readonly IUserProjectService userProjectService;

        public UserProjectsController(IUserProjectService userProjectService)
        {
            this.userProjectService = userProjectService;
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
