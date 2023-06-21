using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.AirTableSync
{
    public interface IAirTableSyncService
    {
        public Task<SyncEvent> ManualAirtableSync(string projectId);
        public Task<SyncEvent> AutomaticAirtableSync(string projectId, string eventId);
    }
}
