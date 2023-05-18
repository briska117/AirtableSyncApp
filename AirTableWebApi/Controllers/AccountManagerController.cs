using AirTableIdentity.Models;
using AirTableWebApi.Configurations;
using AirTableWebApi.Services.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirTableWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountManagerController : ControllerBase
    {
        private readonly IAccountManagerService accountManager;

        public AccountManagerController(IAccountManagerService accountManager)
        {
            this.accountManager = accountManager;
        }

        [HttpGet]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUsers()
        {
            var result = await this.accountManager.GetAllUsers();
            return Ok(result);  
        }
        [HttpGet("GetUserByEmail/{email}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserByEmail([FromRoute] string email)
        {
            var exist = await this.accountManager.UserExistByEmail(email);
            if (!exist) {
                return NotFound();
            }
            var result = await this.accountManager.GetUserByEmail(email);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CreateAccount([FromBody]AccountForm account)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (await this.accountManager.UserExistByEmail(account.Email)) {
                return Conflict($"User {account.Email} already exist!");
            }

            var createUser = await this.accountManager.CreateUser(account);

            return Ok(createUser);
        }


        [HttpGet("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetUserById([FromRoute] string id)
        {
            var exist = await this.accountManager.UserExistById(id);
            if (!exist)
            {
                return NotFound();
            }
            var result = await this.accountManager.GetUserById(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(
            Policy = IdentitySettings.AdminRightsPolicyName,
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteUserById([FromRoute] string id)
        {
            var exist = await this.accountManager.UserExistById(id);
            if (!exist)
            {
                return NotFound();
            }
            var result = await this.accountManager.DeleteUserById(id);
            return Ok(result);
        }


    }
}
