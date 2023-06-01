using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.SyncEvents
{
    public interface ISyncEventsService
    {
        public Task<List<SyncEvent>> GetSyncEvents();
        public Task<SyncEvent> GetSyncEvent(string id);
        public Task<List<SyncEvent>> GetProjectAsyncEvent(string projectId);
        public Task<SyncEvent> AddAsyncEvent(SyncEvent asyncEvent);
        public Task<SyncEvent> UpdateAsyncEvent(SyncEvent asyncEvent);
        public Task<SyncEventHistory> AddSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<SyncEventHistory> GetSyncEventHistory(string id);
        public Task<SyncEventHistory> UpdateSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<bool> RemoveAsyncEvent(string id);
        public Task<List<SyncEvent>> GetSyncEventsByProject(string projectId);
        public Task<List<SyncEventHistory>> GetEventHistoryByEventtId(string eventId);
        public Task<List<SyncEventsView>> GetProjectSyncEventFull(string projectId);
    }
}
