using AirTableIdentity;
using AirTableIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AirTableWebApi.Services.Initializations
{
    public static class IdentityInitializer
    {

        /// <summary>Identities the database context initializer.</summary>
        /// <param name="app">The application.</param>
        public static void IdentityDbContextInitializer(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    Migrate(services);

                    var applicationDbContext = services.GetRequiredService<IdentityContext>();
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    if (!IsInitialized(applicationDbContext))
                    {
                        Initialize(userManager, roleManager)
                            .GetAwaiter()
                            .GetResult();
                    }
                }
                catch (Exception exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Failed to seed database");
                }
            }
        }

        /// <summary>Migrates the specified service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        private static void Migrate(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<IdentityContext>();
            context.Database.Migrate();
        }

        /// <summary>Determines whether the specified application database context is initialized.</summary>
        /// <param name="_applicationDbContext">The application database context.</param>
        /// <returns>
        ///   <c>true</c> if the specified application database context is initialized; otherwise, <c>false</c>.</returns>
        private static bool IsInitialized(IdentityContext _applicationDbContext)
        {
            var userCount = _applicationDbContext.Users.Count();
            var roleCount = _applicationDbContext.Roles.Count();

            return 0 < userCount && 3 == roleCount;
        }

        /// <summary>Initializes the specified user manager.</summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        private static async Task Initialize(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await EnsureRole(roleManager, Role.Administrator.ToString());
            await EnsureRole(roleManager, Role.Customer.ToString());
            // Ensure root user
            var user = new IdentityUser { Email = "test@voxelmaps.com", UserName = "test@voxelmaps.com" };
            var identityResult = await userManager.CreateAsync(user, "Pass.word1");
            ThrowIfFailedIdentityResult(identityResult);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            identityResult = await userManager.ConfirmEmailAsync(user, token);
            ThrowIfFailedIdentityResult(identityResult);

            identityResult = await userManager.AddToRoleAsync(user, Role.Administrator.ToString());
            ThrowIfFailedIdentityResult(identityResult);
        }

        /// <summary>
        /// Ensures the role.
        /// </summary>
        /// <param name="roleManager">The role manager.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        private static async Task EnsureRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            bool exists = await roleManager.RoleExistsAsync(roleName);
            if (!exists)
            {
                var role = new IdentityRole { Name = roleName };
                var identityResult = await roleManager.CreateAsync(role);
                ThrowIfFailedIdentityResult(identityResult);
            }
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
