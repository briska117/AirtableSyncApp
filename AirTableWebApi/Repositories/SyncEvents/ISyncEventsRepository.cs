using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.SyncEvents
{
    public interface ISyncEventsRepository
    {
        public Task<List<SyncEvent>> GetSyncEvents();
        public Task<List<SyncEvent>> GetSyncEventsByProject(string projectId);
        public Task<SyncEvent> GetSyncEvent(string id);
        public Task<SyncEvent> AddSyncEvent(SyncEvent asyncEvent);
        public Task<SyncEvent> UpdateSyncEvent(SyncEvent asyncEvent);
        public Task<bool> RemoveSyncEvent(string id);
        public Task<SyncEventHistory> AddSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<SyncEventHistory> UpdateSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<SyncEventHistory> GetSyncEventHistory(string id); 
        public Task<List<SyncEventHistory>> GetEventHistoryByEventId(string eventId);



    }
}
