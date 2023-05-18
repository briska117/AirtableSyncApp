using AirTableDatabase.DBModels;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.CountryPrefixes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryPrefixesController : ControllerBase
    {
        private readonly ICountryPrefixService countryPrefixService;

        public CountryPrefixesController(ICountryPrefixService countryPrefixService)
        {
            this.countryPrefixService = countryPrefixService;
        }
        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCountryPrefixes()
        {
            var countryPrefixes = await countryPrefixService.GetCountryPrefixes();  
            return Ok(countryPrefixes);
        }
        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.CustomerRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetCountryPrefix([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var exist = await countryPrefixService.ExistCountryPrefix(id);
            if (!exist)
            {
                return NotFound();
            }
            var countryPrefixes = await countryPrefixService.GetCountryPrefix(id);
            return Ok(countryPrefixes);
        }
        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> AddCountryPrefix([FromBody] CountryPrefix countryPrefix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this.countryPrefixService.AddCountryPrefix(countryPrefix);

            return Ok(countryPrefix);
        }
        [HttpPut]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateCountryPrefix([FromBody] CountryPrefix countryPrefix)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var exist = await countryPrefixService.ExistCountryPrefix(countryPrefix.CountryPrefixId);
            if (!exist)
            {
                return NotFound();
            }
            await this.countryPrefixService.UpdateCountryPrefix(countryPrefix);

            return Ok(countryPrefix);
        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteCountryPrefix([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            var exist = await countryPrefixService.ExistCountryPrefix(id);
            if (!exist)
            {
                return NotFound();
            }
            var countryPrefixes = await countryPrefixService.RemoveCountryPrefix(id);
            return Ok(countryPrefixes);
        }



    }
}
