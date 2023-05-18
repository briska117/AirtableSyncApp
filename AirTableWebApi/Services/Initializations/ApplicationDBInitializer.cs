using AirTableDatabase;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Services.Initializations
{
    public static class ApplicationDBInitializer
    {
        /// <summary>Migrates the specified service provider.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        private static void Migrate(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDBContext>();
            context.Database.Migrate();
        }
        public static void ApplicationDBContextInitializer(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    Migrate(services);

                }
                catch (Exception exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Failed to seed database");
                }
            }
        }
    }
}
