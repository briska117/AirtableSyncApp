using System.Security.Claims;

namespace AirTableWebApi.Configurations
{
    public static class IdentityPolicyConfig
    {
        public static void InitializePolicyConfig(WebApplicationBuilder? builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(IdentitySettings.ElevatedRightsPolicyName, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, IdentitySettings.GlobalAdministrator);
                    policy.RequireRole(IdentitySettings.GlobalAdministrator);
                });

                options.AddPolicy(IdentitySettings.AdminRightsPolicyName, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, IdentitySettings.GlobalAdministrator, IdentitySettings.Administrator);
                    policy.RequireRole(IdentitySettings.GlobalAdministrator, IdentitySettings.Administrator);
                });

                options.AddPolicy(IdentitySettings.CustomerRightsPolicyName, policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, IdentitySettings.GlobalAdministrator, IdentitySettings.Administrator, IdentitySettings.Customer);
                    policy.RequireRole(IdentitySettings.GlobalAdministrator, IdentitySettings.Administrator, IdentitySettings.Customer);
                });
            });

        }

    }
}
