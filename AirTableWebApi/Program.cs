using AirTableDatabase;
using AirTableIdentity;
using AirTableWebApi.Configurations;
using AirTableWebApi.Repositories.SyncEvents;
using AirTableWebApi.Repositories.ClientPrefixes;
using AirTableWebApi.Repositories.CollectionModes;
using AirTableWebApi.Repositories.CountryPrefixes;
using AirTableWebApi.Repositories.Projects;
using AirTableWebApi.Repositories.RelatedTables;
using AirTableWebApi.Repositories.UserProjects;
using AirTableWebApi.Services.Account;
using AirTableWebApi.Services.AirTableSync;
using AirTableWebApi.Services.AsyncEvents;
using AirTableWebApi.Services.Auth;
using AirTableWebApi.Services.ClientPrefixes;
using AirTableWebApi.Services.CollectionModes;
using AirTableWebApi.Services.CountryPrefixes;
using AirTableWebApi.Services.Initializations;
using AirTableWebApi.Services.Projects;
using AirTableWebApi.Services.RelatedTables;
using AirTableWebApi.Services.UserProjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string? connectionString = builder.Configuration.GetConnectionString("AirTableConnection");
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(connectionString),ServiceLifetime.Singleton);

string? connectionIdentity = builder.Configuration.GetConnectionString("IdentityConnection");
builder.Services.AddDbContext<IdentityContext>(options => options.UseNpgsql(connectionIdentity));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, UserClaimsPrincipalFactory<IdentityUser>>();

//Start Repositories
builder.Services.AddTransient<IProjectsRepository, ProjectsRepository>();
builder.Services.AddTransient<IClientPrefixRepository, ClientPrefixRepository>();
builder.Services.AddTransient<ICountryPrefixRepository, CountryPrefixRepository>();
builder.Services.AddTransient<ICollectionModeRepository, CollectionModeRepository>();
builder.Services.AddTransient<ISyncEventsRepository, SyncEventsRepository>();
builder.Services.AddTransient<IRelatedTableRepository, RelatedTableRepository>();
builder.Services.AddTransient<IUserProjectRepository, UserProjectRepository>();

//Start Services
builder.Services.AddTransient<IProjectsService, ProjectsService>();
builder.Services.AddTransient<IAccountManagerService, AccountManagerService>();
builder.Services.AddTransient<IClientPrefixService, ClientPrefixService>();
builder.Services.AddTransient<ICountryPrefixService, CountryPrefixService>();
builder.Services.AddTransient<ICollectionModeService, CollectionModeService>();
builder.Services.AddTransient<IAsyncEventsService, AsyncEventsService>();
builder.Services.AddTransient<IRelatedTablesService, RelatedTablesService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAirTableSyncService, AirTableSyncService>();
builder.Services.AddTransient<IUserProjectService, UserProjectService>();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Configure JWT Autentication
IdentityJWTAuthenticationConfig.InitializeJWTBearerConfig(builder);
//Configure Identity Role Policy
IdentityPolicyConfig.InitializePolicyConfig(builder);
//Configure Swagger UI JWT
IdentitySwaggerConfig.InitializeSwaggerConfig(builder);

var app = builder.Build();

//Initializations  Initializer
IdentityInitializer.IdentityDbContextInitializer(app);
ApplicationDBInitializer.ApplicationDBContextInitializer(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
