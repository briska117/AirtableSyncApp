using AirTableIdentity.Models;
using Microsoft.AspNetCore.Identity;

namespace AirTableWebApi.Services.Account
{
    public interface IAccountManagerService
    {
        public Task<IdentityUserDTO> CreateUser(AccountForm user);
        public Task<List<IdentityUserDTO>> GetAllUsers();
        public Task<IdentityUserDTO> GetUserByEmail(string email);
        public Task<IdentityUserDTO> GetUserById(string id);
        public Task<bool> UserExistByEmail(string email);
        public Task<bool> UserExistById(string id);
        public Task<bool> DeleteUserById(string id);
    }
}
