using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.AirTableSync
{
    public interface IAirTableSyncService
    {
        public Task<SyncEvent> ManualAirtableSync(string projectId);
    }
}
