using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AirTableWebApi.Configurations
{
    public static class IdentityJWTAuthenticationConfig
    {
        public static void InitializeJWTBearerConfig(WebApplicationBuilder? builder)
        {
            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            }
                );
        }
    }
}
