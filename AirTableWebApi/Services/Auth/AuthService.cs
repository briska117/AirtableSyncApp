using AirTableWebApi.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AirTableWebApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly JwtSettings jwtSettingsOptions;

        public AuthService(UserManager<IdentityUser> userManager, IOptions<JwtSettings> jwtSettingsOptions)
        {
            this.userManager = userManager;
            this.jwtSettingsOptions = jwtSettingsOptions.Value;
        }
        public async Task<IdentityUser> Authenticate(string email, string password)
        {
            IdentityUser user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            bool successPassword = await this.userManager.CheckPasswordAsync(user, password);

            if (!successPassword)
            {
                return null;
            }

            return user;
        }

        public async Task<string> GenerateToken(IdentityUser applicationUser)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettingsOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userRoles = await this.userManager.GetRolesAsync(applicationUser);

            var role = userRoles.FirstOrDefault();

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, this.jwtSettingsOptions.Issuer),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: this.jwtSettingsOptions.Issuer,
                audience: this.jwtSettingsOptions.Issuer,
                claims: claims,
                expires: DateTime.Now.AddHours(this.jwtSettingsOptions.AccessExpiration),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
