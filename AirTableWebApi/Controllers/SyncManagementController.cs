using AirTableWebApi.Configurations;
using AirTableWebApi.Services.AirTableSync;
using AirTableWebApi.Services.Projects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Navmii.AirtableSync;
using System.Diagnostics;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncManagementController : ControllerBase
    {
        private readonly IProjectsService projectsService;
        private readonly IAirTableSyncService syncService;

        public SyncManagementController(IProjectsService projectsService, IAirTableSyncService syncService)
        {
            this.projectsService = projectsService;
            this.syncService = syncService;
        }

        [HttpGet("SyncProject/{projectId}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> SyncProject(string projectId)
        {
            try
            {
                var project = this.projectsService.GetProject(projectId);
                if(project == null)
                {
                    return NotFound();
                }

                var result = await this.syncService.ManualAirtableSync(projectId);
               

                return Ok(new { Success = true, Message = $"Sync start now  {DateTime.UtcNow}" });

            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
