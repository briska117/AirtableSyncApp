using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.CollectionModes;
using AirTableWebApi.Services.RelatedTables;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionModeController : ControllerBase
    {
        private readonly ICollectionModeService collectionMode;
        private readonly IRelatedTablesService relatedTablesService;

        public CollectionModeController(ICollectionModeService collectionMode, IRelatedTablesService relatedTablesService)
        {
            this.collectionMode = collectionMode;
            this.relatedTablesService = relatedTablesService;
        }

        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCollectionModesAsync()
        {
            var collectionModes = await this.collectionMode.GetCollectionModes();
            var response = new List<CollectionModeResponse>();

            foreach (var collectionMode in collectionModes)
            {
                List<CollectionModeRelatedTable> collectionModeRelatedTables = await this.collectionMode.GetCollectionModeRelatedTable(collectionMode.CollectionModeId);
                List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
                List<String> ids = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToList();
                relatedTables = relatedTables.Where(r => ids.Contains(r.RelatedTableId)).ToList();
                CollectionModeResponse collectionModeResponse = new CollectionModeResponse();
                collectionModeResponse.CollectionMode = collectionMode;
                collectionModeResponse.relatedTables = relatedTables;
                response.Add(collectionModeResponse);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCollectionModeAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var collectionMode = await this.collectionMode.GetCollectionMode(id);
            if(collectionMode == null)
            {
                return NotFound();
            }
            List<CollectionModeRelatedTable> collectionModeRelatedTables = await this.collectionMode.GetCollectionModeRelatedTable(id);
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            List<String> ids = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToList();
            relatedTables = relatedTables.Where(r => ids.Contains(r.RelatedTableId)).ToList();
            foreach (var item in relatedTables)
            {
                var info = collectionModeRelatedTables.FirstOrDefault(c=>c.RelatedTableId==item.RelatedTableId);
                item.TeamList = info.IsTeam;
                item.MainList = info.IsMain;    
            }
            return Ok(new {collectionMode=collectionMode,relatedTables=relatedTables});
        }

        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CollectionMode>> AddCollectionModeAsync(CollectionModeRequest collectionModeRequest)
        {
            if (!ModelState.IsValid) { 
                return BadRequest();
            }
            try
            {
                foreach (RelatedTableDto table in collectionModeRequest.RelatedTables)
                {
                    var validId =await this.relatedTablesService.GetRelatedTable(table.TableId);
                    if (validId == null)
                    {
                        return NotFound($"not fount related table with id {table.TableId}");
                    }
                }
                var collectionMode = await this.collectionMode.AddCollectionMode(collectionModeRequest.CollectionMode);
                if (collectionModeRequest.TablesHaveChange)
                {
                    await AddOrUpdateRelatedTables(collectionModeRequest.RelatedTables, collectionMode.CollectionModeId);
                }

                 var response = await this.GetCollectionModeAsync(collectionMode.CollectionModeId);
                //Assert
                var result = response as OkObjectResult;
                return Ok(result.Value);
            }catch(Exception ex)
            {
                return Conflict(ex);    
            }
        }

        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CollectionMode>> UpdateCollectionModeAsync(CollectionModeRequest collectionModeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            foreach (RelatedTableDto table in collectionModeRequest.RelatedTables)
            {
                var validId =await this.relatedTablesService.GetRelatedTable(table.TableId);
                if (validId == null)
                {
                    return NotFound($"not fount related table with id {table.TableId}");
                }
            }
            var collectionModeExist = await this.collectionMode.GetCollectionMode(collectionModeRequest.CollectionMode.CollectionModeId);
            if (collectionModeExist == null)
            {
                return NotFound();
            }
           var collectionMode = await this.collectionMode.UpdateCollectionMode(collectionModeRequest.CollectionMode);

            if (collectionModeRequest.TablesHaveChange)
            {
                await AddOrUpdateRelatedTables(collectionModeRequest.RelatedTables, collectionMode.CollectionModeId);
            }

            var response = await this.GetCollectionModeAsync(collectionModeRequest.CollectionMode.CollectionModeId);


            //Assert
            var result = response as OkObjectResult;
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CollectionMode>> DeleteCollectionModeAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var collectionMode = await this.collectionMode.GetCollectionMode(id);
            if (collectionMode == null)
            {
                return NotFound();
            }
            var delete = await this.collectionMode.DeleteCollectionMode(id);
            return Ok(delete );
        }

        [HttpPost("AddTableToCollectionMode")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddTableToCollectionMode(CollectionRelatedRequest modeRelatedRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var collectionMode = await this.collectionMode.GetCollectionMode(modeRelatedRequest.CollectionModeId);
            if (collectionMode == null)
            {
                return NotFound();
            }

            foreach (string id in modeRelatedRequest.RelatedTablesIds)
            {
                var validId = this.relatedTablesService.GetRelatedTable(id);
                if(validId == null)
                {
                    return NotFound($"not fount related table with id {id}");
                }
            }
            foreach (string id in modeRelatedRequest.RelatedTablesIds)
            {
                CollectionModeRelatedTable collectionModeRelatedTable = new CollectionModeRelatedTable
                {
                    CollectionModeId = modeRelatedRequest.CollectionModeId,
                    RelatedTableId = id
                };
                await this.collectionMode.AddCollectionModeRelatedTable(collectionModeRelatedTable);
            }
            return Ok();
        }
        [HttpGet("GetCollectionModeRelatedTables/{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<RelatedTable>>> GetCollectionModeRelatedTables([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var collectionMode = await this.collectionMode.GetCollectionMode(id);
            if (collectionMode == null)
            {
                return NotFound();
            }
            List<CollectionModeRelatedTable> collectionModeRelatedTables = await this.collectionMode.GetCollectionModeRelatedTable(id);
            List<RelatedTable> relatedTables = await this.relatedTablesService.GetRelatedTables();
            List<String> ids = collectionModeRelatedTables.Select(c => c.RelatedTableId).ToList();
            relatedTables = relatedTables.Where(r=> ids.Contains(r.RelatedTableId)).ToList();

            return Ok(relatedTables);
        }
    
    
        private async Task<List<CollectionModeRelatedTable>> AddOrUpdateRelatedTables(List<RelatedTableDto> relatedTables, string CollectionModeId) {
            List<CollectionModeRelatedTable> result = new List<CollectionModeRelatedTable>();
            var existItems = await this.collectionMode.GetCollectionModeRelatedTable(CollectionModeId);
            await this.collectionMode.DeleteRangeCollectionModeRelatedTable(existItems);
            foreach (RelatedTableDto table in relatedTables)
            {
                CollectionModeRelatedTable collectionModeRelatedTable = new CollectionModeRelatedTable
                {
                    CollectionModeId = CollectionModeId,
                    RelatedTableId = table.TableId,
                    IsMain = table.IsMain,
                    IsTeam = table.IsTeam

                };
                await this.collectionMode.AddCollectionModeRelatedTable(collectionModeRelatedTable);
                result.Add(collectionModeRelatedTable); 
            }
            return result;  

        }
    
    
    }
}
