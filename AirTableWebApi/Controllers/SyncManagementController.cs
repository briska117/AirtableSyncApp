using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.AirTableApi;
using AirTableWebApi.Services.AirTableSync;
using AirTableWebApi.Services.Projects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Navmii.AirtableSync;
using Navmii.AirTableSyncNetcore6.AirtablesModels;
using System.Diagnostics;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncManagementController : ControllerBase
    {
        private readonly IProjectsService projectsService;
        private readonly IAirTableSyncService syncService;
        private readonly IAirTableApiService airTableApiService;

        public SyncManagementController(IProjectsService projectsService, IAirTableSyncService syncService, IAirTableApiService airTableApiService)
        {
            this.projectsService = projectsService;
            this.syncService = syncService;
            this.airTableApiService = airTableApiService;
        }

        [HttpGet("SyncProject/{projectId}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> SyncProject(string projectId)
        {
            try
            {
                var project =await this.projectsService.GetProject(projectId);
                if (project == null)
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

        [HttpPost("GetDatabases")]
        public async Task<ActionResult<BasisResponse>> GetDatabases([FromBody] AirTableApiSettings airTableApiSettings )
        {
            try
            {
                if (string.IsNullOrEmpty(airTableApiSettings.AccessToken))
                {
                    return BadRequest("api token could not be empty");
                }
                var response =await this.airTableApiService.GetDatabases(airTableApiSettings);
                return Ok(response);    

            }catch(Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("GetTables")]
        public async Task<ActionResult<TablesResponse>> GetTables([FromBody] AirTableApiSettings airTableApiSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(airTableApiSettings.AccessToken))
                {
                    return BadRequest("api token could not be empty");
                }
                if (string.IsNullOrEmpty(airTableApiSettings.DataBaseId))
                {
                    return BadRequest("Database Id could not be empty");
                }
                var response = await this.airTableApiService.GetTables(airTableApiSettings);
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("GetRecords")]
        public async Task<ActionResult<RecordResponse>> GetRecords([FromBody] AirTableApiSettings airTableApiSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(airTableApiSettings.AccessToken))
                {
                    return BadRequest("api token could not be empty");
                }
                if (string.IsNullOrEmpty(airTableApiSettings.DataBaseId))
                {
                    return BadRequest("Database Id could not be empty");
                }
                if (string.IsNullOrEmpty(airTableApiSettings.TableId))
                {
                    return BadRequest("Table Id could not be  empty");
                }
                var response = await this.airTableApiService.GetRecords(airTableApiSettings);
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("GetFirstRecord")]
        public async Task<ActionResult<Record>> GetFirstRecord([FromBody] AirTableApiSettings airTableApiSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(airTableApiSettings.AccessToken))
                {
                    return BadRequest("api token could not be empty");
                }
                if (string.IsNullOrEmpty(airTableApiSettings.DataBaseId))
                {
                    return BadRequest("Database Id could not be empty");
                }
                if (string.IsNullOrEmpty(airTableApiSettings.TableId))
                {
                    return BadRequest("Table Id could not be  empty");
                }
                var response = await this.airTableApiService.GetFirstRecord(airTableApiSettings);
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("GetProjectDatabaseSchema")]
        public async Task<ActionResult<AirTablesInfo>> GetProjectDatabaseSchema([FromBody] AirTableApiSettings airTableApiSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(airTableApiSettings.AccessToken))
                {
                    return BadRequest("api token could not be empty");
                }
                if (string.IsNullOrEmpty(airTableApiSettings.DataBaseId))
                {
                    return BadRequest("Database Id could not be empty");
                }
                
                var response = await this.airTableApiService.GetProjectDatabaseSchema(airTableApiSettings);
                return Ok(response);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
