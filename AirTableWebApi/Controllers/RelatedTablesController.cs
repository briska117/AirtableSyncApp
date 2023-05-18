using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.RelatedTables;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelatedTablesController : ControllerBase
    {
        private readonly IRelatedTablesService relatedTablesService;

        public RelatedTablesController(IRelatedTablesService relatedTablesService)
        {
            this.relatedTablesService = relatedTablesService;
        }
        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetRelatedTables()
        {
            var result =await this.relatedTablesService.GetRelatedTables();
            return Ok(result);  
        }
        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetRelatedTable([FromRoute]string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();    
            }
            var result =await this.relatedTablesService.GetRelatedTable(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddRelatedTable([FromBody]RelatedTable relatedTable)
        {
            if (!ModelState.IsValid){
                return BadRequest();
            }
            await this.relatedTablesService.AddRelatedTable(relatedTable);
            return Ok(relatedTable);

        }

        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateRelatedTable([FromBody] RelatedTable relatedTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var exist = this.relatedTablesService.GetRelatedTable(relatedTable.RelatedTableId); 
            if(exist == null)
            {
                return NotFound();  
            }
            await this.relatedTablesService.UpdateRelatedTable(relatedTable);
            return Ok(relatedTable);

        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RemoveRelatedTable([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var exist = this.relatedTablesService.GetRelatedTable(id);
            if (exist == null)
            {
                return NotFound();
            }
            var result = await this.relatedTablesService.RemoveRelatedTable(id);
            return Ok(result);
        }
    }
}
