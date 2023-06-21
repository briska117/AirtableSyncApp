using AirTableDatabase.DBModels;
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

        [HttpPost("AutomaticSyncProjects")]
        public async Task<ActionResult> AutomaticSyncProjects([FromBody]List<AutomaticSyncProject> syncProjects)
        {
            try
            {
                List<SyncEvent> syncEvents = new List<SyncEvent>(); 
                if(syncProjects == null || syncProjects.Count() < 1)
                {
                    return BadRequest();  
                }
                foreach (var syncProject in syncProjects)
                {
                    var applySync= await this.syncService.AutomaticAirtableSync(syncProject.ProjectId,syncProject.EventId);
                    syncEvents.Add(applySync);  
                }
                return Ok(new { Success = true, Message = $"Sync start now  {DateTime.UtcNow}", events = syncProjects });
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
