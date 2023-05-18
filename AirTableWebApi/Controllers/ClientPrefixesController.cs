using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.ClientPrefixes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientPrefixesController : ControllerBase
    {
        private readonly IClientPrefixService clientPrefixService;

        public ClientPrefixesController(IClientPrefixService clientPrefixService)
        {
            this.clientPrefixService = clientPrefixService;
        }


        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetClientPrefixes()
        {
            var clientPrefixes = await clientPrefixService.GetClientPrefixes(); 
            return Ok(clientPrefixes);
        }

        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetClientPrefix([FromRoute]string id)
        {
            if (string.IsNullOrEmpty(id)) {
                return BadRequest();
            }  
            var exist = await clientPrefixService.ExistClientPrefix(id);
            if (!exist)
            {
                return NotFound();
            }
            var clientId = await clientPrefixService.GetClientPrefix(id);
            return Ok(clientId);
        }

        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddClientPrefix([FromBody]ClientPrefix clientPrefix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();    
            }
            await clientPrefixService.AddClientPrefix(clientPrefix);
            return Ok(clientPrefix);    
        }

        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateClientPrefix([FromBody] ClientPrefix clientPrefix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var exist = await clientPrefixService.ExistClientPrefix(clientPrefix.ClientPrefixId);
            if (!exist)
            {
                return NotFound();
            }
            await clientPrefixService.UpdateClientPrefix(clientPrefix);
            return Ok(clientPrefix);
        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteClientPrefix([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var exist = await clientPrefixService.ExistClientPrefix(id);
            if (!exist)
            {
                return NotFound();
            }
            var clientId = await clientPrefixService.DeleteClientPrefix(id);
            return Ok(clientId);
        }
    }

}
