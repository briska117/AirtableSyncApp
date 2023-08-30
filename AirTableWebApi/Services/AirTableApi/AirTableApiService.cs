using AutoMapper;
using Navmii.AirTableSyncNetcore6.AirtablesModels;
using Navmii.Request;
using System.ComponentModel;

namespace AirTableWebApi.Services.AirTableApi
{
    public class AirTableApiService : IAirTableApiService
    {
        private readonly IRequestService requestService;
        private readonly IMapper mapper;
        private readonly string baseUrl = "https://api.airtable.com/v0";

        public AirTableApiService(IRequestService requestService, IMapper mapper) {
            this.requestService = requestService;
            this.mapper = mapper;
        } 
        public async Task<BasisResponse> GetDatabases(AirTableApiSettings airTableApiSettings)
        {
            try
            {
                string url = $"{this.baseUrl}/meta/bases";
                BasisResponse basisResponse = await this.requestService.GetAsync<BasisResponse>(
                    url, 
                    AuthenticationScheme.Bearer, 
                    airTableApiSettings.AccessToken
                    );
                return basisResponse;   

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            
        }

        public async Task<Record> GetFirstRecord(AirTableApiSettings airTableApiSettings)
        {
            try
            {
                RecordResponse recordResponse = await this.GetRecords(airTableApiSettings);
                Record record = recordResponse.Records.FirstOrDefault();
                return record;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<AirTablesInfo> GetProjectDatabaseSchema(AirTableApiSettings airTableApiSettings)
        {
            try
            {
                AirTablesInfo airTablesInfo = new AirTablesInfo();
                BasisResponse basisResponse = await GetDatabases(airTableApiSettings);
                airTablesInfo.CentralDataBase = basisResponse.Bases.FirstOrDefault(b => b.Id == airTableApiSettings.DataBaseId);
                TablesResponse tablesResponse = await GetTables(airTableApiSettings);
                List<TableDto> tablesCentral = this.mapper.Map<List<TableDto>>(tablesResponse.Tables);
                TableDto teamsTable = tablesCentral.FirstOrDefault(t => t.Name == "Teams");
                if (teamsTable == null)
                {
                    throw new Exception($"Database with ID {airTableApiSettings.DataBaseId}");
                }
                airTablesInfo.CentralTables = tablesCentral;
                airTableApiSettings.TableId = teamsTable.Id;
                RecordResponse recordResponse = await GetRecords(airTableApiSettings);
                List<TeamsDto> teams = await GetTeamsInfo(recordResponse.Records);
                foreach (TeamsDto team in teams)
                {
                    airTableApiSettings.DataBaseId = team.DataBaseId;
                    team.TeamTables = await GetTeamTables(airTableApiSettings);
                }
                airTablesInfo.Teams = teams;
                return airTablesInfo;
            }
            catch (Exception ex)
            {

                throw new Exception("");
            }
        }
         private async Task<List<TableDto>> GetTeamTables(AirTableApiSettings airTableApiSettings) {
            try
            {
                var tableResponse = await GetTables(airTableApiSettings);
                List<TableDto> teamTables = this.mapper.Map<List<TableDto>>(tableResponse.Tables);
                return teamTables;  
            }
            catch (Exception ex)
            {

                throw;
            }
        
        }

        private async Task<List<TeamsDto>> GetTeamsInfo(List<Record> records ) {
            try
            {
                List<TeamsDto> teams = new List<TeamsDto>();
                foreach (var item in records)
                {
                    TeamsDto team = new TeamsDto();
                    var teamName = item.GetField("Name");
                    var teamId = item.GetField("DatabaseID");
                    if (teamName != null && teamId != null) {
                        team.Name = teamName.ToString();
                        team.DataBaseId = teamId.ToString();
                        teams.Add(team);    
                    }

                }

                
                return teams;   
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<RecordResponse> GetRecords(AirTableApiSettings airTableApiSettings)
        {
            try
            {
                string url = $"{this.baseUrl}/{airTableApiSettings.DataBaseId}/{airTableApiSettings.TableId}";
                RecordResponse recordResponse = await this.requestService.GetAsync<RecordResponse>(
                    url,
                    AuthenticationScheme.Bearer,
                    airTableApiSettings.AccessToken
                    );
                return recordResponse;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<TablesResponse> GetTables(AirTableApiSettings airTableApiSettings)
        {
            try
            {
                string url = $"{this.baseUrl}/meta/bases/{airTableApiSettings.DataBaseId}/tables";
                TablesResponse tablesResponse = await this.requestService.GetAsync<TablesResponse>(
                    url,
                    AuthenticationScheme.Bearer,
                    airTableApiSettings.AccessToken
                    );
                return tablesResponse;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
