using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.AsyncEvents
{
    public interface IAsyncEventsService
    {
        public Task<List<SyncEvent>> GetAsyncEvents();
        public Task<SyncEvent> GetAsyncEvent(string id);
        public Task<List<SyncEvent>> GetProjectAsyncEvent(string projectId);
        public Task<SyncEvent> AddAsyncEvent(SyncEvent asyncEvent);
        public Task<SyncEvent> UpdateAsyncEvent(SyncEvent asyncEvent);
        public Task<SyncEventHistory> AddSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<SyncEventHistory> GetSyncEventHistory(string id);
        public Task<SyncEventHistory> UpdateSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<bool> RemoveAsyncEvent(string id);
    }
}
