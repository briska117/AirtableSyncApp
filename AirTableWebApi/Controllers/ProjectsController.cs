using AirTableDatabase;
using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.AsyncEvents;
using AirTableWebApi.Services.ClientPrefixes;
using AirTableWebApi.Services.CollectionModes;
using AirTableWebApi.Services.CountryPrefixes;
using AirTableWebApi.Services.Projects;
using AirTableWebApi.Services.RelatedTables;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService projectsService;
        private readonly IAsyncEventsService asyncEventsService;
        private readonly ICollectionModeService collectionMode;
        private readonly IRelatedTablesService relatedTablesService;
        private readonly IClientPrefixService clientPrefixesService;
        private readonly ICountryPrefixService countryPrefixesService;
        private readonly IMapper mapper;

        public ProjectsController(
            IProjectsService projectsService,
            IAsyncEventsService asyncEventsService,
            ICollectionModeService collectionMode,
            IRelatedTablesService relatedTablesService,
            IClientPrefixService clientPrefixesService,
            ICountryPrefixService countryPrefixesService,
            IMapper mapper
            )
        {
            this.projectsService = projectsService;
            this.asyncEventsService = asyncEventsService;
            this.collectionMode = collectionMode;
            this.relatedTablesService = relatedTablesService;
            this.clientPrefixesService = clientPrefixesService;
            this.countryPrefixesService = countryPrefixesService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<List<Project>> GetProjects()
        {
            var result = await this.projectsService.GetProjects();
            return result;
        }
        [HttpGet("{projectId}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ProjectResponse>> GetProject([FromRoute]string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest();
            }
            if (!await this.projectsService.ProjectExist(projectId))
            {
                return NotFound($"Not found project with CountryPrefixId {projectId}");
            }
            var projects = await this.projectsService.GetProject(projectId);
            var collectionMode = await this.collectionMode.GetCollectionMode(projects.Mode);
            if (collectionMode == null)
            {
                return NotFound($"collection mode {projects.Mode} dont exist");
            }
            List<CollectionModeRelatedTable> collectionModeRelatedTables = await this.collectionMode.GetCollectionModeRelatedTable(collectionMode.CollectionModeId);
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            List<String> ids = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToList();
            relatedTables = relatedTables.Where(r => ids.Contains(r.RelatedTableId)).ToList();
            var events = await this.asyncEventsService.GetProjectAsyncEvent(projectId);
            
            ProjectResponse result = new ProjectResponse
            {
                Project = projects,
                CollectionMode = collectionMode,
                RelatedTables = relatedTables,
                Events = events
            };

            return result;
        }


        [HttpGet("Extend/{projectId}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetProjectExtend([FromRoute] string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest();
            }
            if (!await this.projectsService.ProjectExist(projectId))
            {
                return NotFound($"Not found project with CountryPrefixId {projectId}");
            }
            var project = await this.projectsService.GetProjectExtend(projectId);
          
            return Ok(project);
        }

        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateProject([FromBody] ProjectRequest projectRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Project project = this.mapper.Map<Project>(projectRequest.Project); 
            await this.projectsService.AddProject(project); 
            foreach(SyncEvent asyncEvent in projectRequest.Events)
            {
                asyncEvent.ProjectId = project.ProjectId;
                await this.asyncEventsService.AddAsyncEvent(asyncEvent);    
            }
            return Ok(projectRequest);
        }
        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateProject([FromBody] ProjectRequest projectRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (! await this.projectsService.ProjectExist(projectRequest.Project.ProjectId))
            {
                return NotFound($"Not found project with CountryPrefixId {projectRequest.Project.ProjectId}");
            }
            Project project = this.mapper.Map<Project>(projectRequest.Project);
            await this.projectsService.UpdateProject(project);
            foreach (SyncEvent asyncEvent in projectRequest.Events)
            {
                var exist = await this.asyncEventsService.GetAsyncEvent(asyncEvent.SyncEventId);
                if (exist == null)
                {
                    asyncEvent.ProjectId = projectRequest.Project.ProjectId;
                    await this.asyncEventsService.AddAsyncEvent(asyncEvent);
                }
                else
                {
                    await this.asyncEventsService.UpdateAsyncEvent(asyncEvent);
                }
                
            }

            return Ok(projectRequest);
        }
        [HttpDelete("{projectId}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteProject([FromRoute] string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest();  
            }
            if (!await this.projectsService.ProjectExist(projectId))
            {
                return NotFound($"Not found project with CountryPrefixId {projectId}");
            }
            await this.projectsService.DeleteProject(projectId);
            return Ok(true);
        }



        [HttpGet("GetProjectView/{projectId}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetProjectView([FromRoute] string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest(ModelState);
            }
            ProjectView view = new ProjectView
            {
                Project = await this.projectsService.GetProjectExtend(projectId),
                CollectionModes = await this.collectionMode.GetCollectionModes(),
                RelatedTables = await this.relatedTablesService.GetRelatedTables(),
                ClientPrefixes = await this.clientPrefixesService.GetClientPrefixes(),
                CountryPrefixes = await this.countryPrefixesService.GetCountryPrefixes()
            };

            return Ok(view);
        }
    }
}
