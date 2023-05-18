using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.AsyncEvents
{
    public interface IAsyncEventsRepository
    {
        public Task<List<SyncEvent>> GetAsyncEvents();
        public Task<SyncEvent> GetAsyncEvent(string id);
        public Task<SyncEvent> AddAsyncEvent(SyncEvent asyncEvent);
        public Task<SyncEvent> UpdateAsyncEvent(SyncEvent asyncEvent);
        public Task<bool> RemoveAsyncEvent(string id);
        public Task<SyncEventHistory> AddSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<SyncEventHistory> UpdateSyncEventHistory(SyncEventHistory asyncEventHistory);
        public Task<SyncEventHistory> GetSyncEventHistory(string id);


    }
}
