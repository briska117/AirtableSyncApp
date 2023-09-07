using AirTableDatabase;
using AirTableIdentity;
using AirTableIdentity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Text;

namespace AirTableWebApi.Services.Account
{
    public class AccountManagerService: IAccountManagerService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IdentityContext identityContext;

        public AccountManagerService(UserManager<IdentityUser> userManager, IdentityContext identityContext)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
        }

        public async Task<IdentityUserDTO> CreateUser(AccountForm accountForm)
        {

            if (await UserExistByEmail(accountForm.Email))
            {
                throw new ArgumentException("User Already exist");
            }


            IdentityUser user = new IdentityUser
            {
                Email = accountForm.Email,
                UserName = accountForm.Email,
                PhoneNumber = accountForm.PhoneNumber  

            };

            var identityResult = await this.userManager.CreateAsync(user, accountForm.Password);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            identityResult = await userManager.ConfirmEmailAsync(user, token);
            ThrowIfFailedIdentityResult(identityResult);
            foreach (Role role in accountForm.Roles)
            {
                identityResult = await userManager.AddToRoleAsync(user, role.ToString());
                ThrowIfFailedIdentityResult(identityResult);
            }

            var resutl = await SetIdentityUserDTO(user); 
            
            return resutl;

        }

        public async Task<List<IdentityUserDTO>> GetAllUsers()
        {
            List<IdentityUser> users = new List<IdentityUser>();
            List<IdentityUserDTO> usersDTO = new List<IdentityUserDTO>();
            users = this.identityContext.Users.ToList();
            foreach (var user in users)
            {
                usersDTO.Add(await SetIdentityUserDTO(user));
            }
            return usersDTO;
        }

        public async Task<IdentityUserDTO> GetUserByEmail(string email)
        {
            IdentityUser user =await this.userManager.FindByEmailAsync(email);
            IdentityUserDTO userDTO = await SetIdentityUserDTO(user);
            return userDTO; 
        }

        private async Task<List<string>> GetUserRoles(IdentityUser user)
        {
            List<string> roles = new List<string>();
            var UserRoles = await this.userManager.GetRolesAsync(user);
            foreach (var role in UserRoles)
            {
                roles.Add(role.ToString());
            } 
            return roles;   

        }

        public async Task<bool> UserExistByEmail(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> UserExistById(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<IdentityUserDTO> GetUserById(string id)
        {
            IdentityUser user = await this.userManager.FindByIdAsync(id);
            IdentityUserDTO userDTO = await SetIdentityUserDTO(user);
           
            return userDTO;
        }


        public async Task<bool> DeleteUserById(string id)
        {

            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new Exception();    
            }
            var delete = await this.userManager.DeleteAsync(user);
            ThrowIfFailedIdentityResult(delete);
            if (!delete.Succeeded)
            {
                return false;
            }
            return true;
        }


        private async Task<IdentityUserDTO> SetIdentityUserDTO(IdentityUser identityUser)
        {
            IdentityUserDTO userDTO = new IdentityUserDTO
            {
                Id = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                NormalizedEmail = identityUser.NormalizedEmail,
                EmailConfirmed = identityUser.EmailConfirmed,
                NormalizedUserName = identityUser.NormalizedUserName,
                ConcurrencyStamp = identityUser.ConcurrencyStamp,
                PhoneNumber = identityUser.PhoneNumber,
                LockoutEnabled = identityUser.LockoutEnabled,
                LockoutEnd = identityUser.LockoutEnd,
                Roles = await GetUserRoles(identityUser),   
            };

            return userDTO;
        }



        /// <summary>
        /// Throws if failed identity result.
        /// </summary>
        /// <param name="identityResult">The identity result.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ThrowIfFailedIdentityResult(IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
            {
                var sb = new StringBuilder();
                foreach (var error in identityResult.Errors)
                {
                    sb.AppendLine($"({error.Code}) {error.Description}");
                }

                throw new InvalidOperationException(sb.ToString());
            }
        }

        
    }
}
