using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.AsyncEvents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncEventsController : ControllerBase
    {
        private readonly IAsyncEventsService eventsService;

        public SyncEventsController(IAsyncEventsService eventsService)
        {
            this.eventsService = eventsService;
        }
        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<SyncEvent>>> GetAsyncEvents()
        {
            return await eventsService.GetAsyncEvents();    
        }

        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SyncEvent>> GetAsyncEvent([FromRoute]string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            return await eventsService.GetAsyncEvent(id);
        }



        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SyncEvent>> AddAsyncEvent([FromBody]SyncEvent asyncEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await this.eventsService.AddAsyncEvent(asyncEvent);
            return asyncEvent;
        }


        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SyncEvent>> UpdateAsyncEvent([FromBody] SyncEvent asyncEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var exist = await this.GetAsyncEvent(asyncEvent.SyncEventId);
            if(exist == null)
            {
                return NotFound();  
            }
            await this.eventsService.UpdateAsyncEvent(asyncEvent);
            return asyncEvent;
        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RemoveAsyncEvent([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var exist = await this.GetAsyncEvent(id);
            if (exist == null)
            {
                return NotFound();
            }
            var result = await this.eventsService.RemoveAsyncEvent(id);  
            return Ok(result);  
        }
    }
}
