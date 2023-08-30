using Navmii.AirTableSyncNetcore6.AirtablesModels;

namespace AirTableWebApi.Services.AirTableApi
{
    public interface IAirTableApiService
    {
        public Task<BasisResponse> GetDatabases(AirTableApiSettings airTableApiSettings);
        public Task<TablesResponse> GetTables(AirTableApiSettings airTableApiSettings);
        public Task<RecordResponse> GetRecords(AirTableApiSettings airTableApiSettings);
        public Task<Record> GetFirstRecord(AirTableApiSettings airTableApiSettings);
        public Task<AirTablesInfo> GetProjectDatabaseSchema(AirTableApiSettings airTableApiSettings);

    }
}
