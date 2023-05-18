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
                foreach (string id in collectionModeRequest.RelatedTables)
                {
                    var validId = this.relatedTablesService.GetRelatedTable(id);
                    if (validId == null)
                    {
                        return NotFound($"not fount related table with id {id}");
                    }
                }
                var collectionMode = await this.collectionMode.AddCollectionMode(collectionModeRequest.CollectionMode);
                foreach (string id in collectionModeRequest.RelatedTables)
                {
                    CollectionModeRelatedTable collectionModeRelatedTable = new CollectionModeRelatedTable
                    {
                        CollectionModeId = collectionMode.CollectionModeId,
                        RelatedTableId = id
                    };
                    await this.collectionMode.AddCollectionModeRelatedTable(collectionModeRelatedTable);
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
            foreach (string id in collectionModeRequest.RelatedTables)
            {
                var validId = this.relatedTablesService.GetRelatedTable(id);
                if (validId == null)
                {
                    return NotFound($"not fount related table with id {id}");
                }
            }
            var collectionModeExist = await this.collectionMode.GetCollectionMode(collectionModeRequest.CollectionMode.CollectionModeId);
            if (collectionModeExist == null)
            {
                return NotFound();
            }
            await this.collectionMode.UpdateCollectionMode(collectionModeRequest.CollectionMode);
            List<CollectionModeRelatedTable> relatedTables = await this.collectionMode.GetCollectionModeRelatedTable(collectionModeRequest.CollectionMode.CollectionModeId);
            foreach (string id in collectionModeRequest.RelatedTables)
            {
                

                var exist = relatedTables.Where(r => r.RelatedTableId == id).FirstOrDefault();
                if (exist == null)
                {
                    CollectionModeRelatedTable collectionModeRelatedTable = new CollectionModeRelatedTable
                    {
                        CollectionModeId = collectionModeRequest.CollectionMode.CollectionModeId,
                        RelatedTableId = id
                    };
                    await this.collectionMode.AddCollectionModeRelatedTable(collectionModeRelatedTable);
                }

                relatedTables.Remove(exist);
            }

            foreach(var relatedTable in relatedTables)
            {
                await this.collectionMode.DeleteCollectionModeRelatedTable(relatedTable.CollectionModeId,relatedTable.RelatedTableId);
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
    }
}
