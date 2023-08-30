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
using AirTableWebApi.Services.SyncEvents;
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
using AirTableWebApi.Services.AirTableFields;
using AirTableWebApi.Repositories.AirTableFields;
using AirTableWebApi.Services.AirTableApi;
using Navmii.Request;

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
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IClientPrefixRepository, ClientPrefixRepository>();
builder.Services.AddScoped<ICountryPrefixRepository, CountryPrefixRepository>();
builder.Services.AddScoped<ICollectionModeRepository, CollectionModeRepository>();
builder.Services.AddScoped<ISyncEventsRepository, SyncEventsRepository>();
builder.Services.AddScoped<IRelatedTableRepository, RelatedTableRepository>();
builder.Services.AddScoped<IUserProjectRepository, UserProjectRepository>();
builder.Services.AddScoped<IAirTableFieldsRepository, AirTableFieldsRepository>();

//Start Services
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IAccountManagerService, AccountManagerService>();
builder.Services.AddScoped<IClientPrefixService, ClientPrefixService>();
builder.Services.AddScoped<ICountryPrefixService, CountryPrefixService>();
builder.Services.AddScoped<ICollectionModeService, CollectionModeService>();
builder.Services.AddScoped<ISyncEventsService, SyncEventsService>();
builder.Services.AddScoped<IRelatedTablesService, RelatedTablesService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAirTableSyncService, AirTableSyncService>();
builder.Services.AddScoped<IUserProjectService, UserProjectService>();
builder.Services.AddScoped<IAirTableFieldsService, AirTableFieldsService>();
builder.Services.AddScoped<IRequestService , RequestService>(); 
builder.Services.AddScoped<IAirTableApiService, AirTableApiService>();


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
