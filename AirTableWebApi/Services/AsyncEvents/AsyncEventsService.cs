using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.SyncEvents;

namespace AirTableWebApi.Services.AsyncEvents
{
    public class AsyncEventsService : IAsyncEventsService
    {
        private readonly ISyncEventsRepository eventsRepository;

        public AsyncEventsService(ISyncEventsRepository eventsRepository)
        {
            this.eventsRepository = eventsRepository;
        }
        public async Task<SyncEvent> AddAsyncEvent(SyncEvent asyncEvent)
        {
            return await eventsRepository.AddSyncEvent(asyncEvent);
        }

        public async Task<SyncEventHistory> AddSyncEventHistory(SyncEventHistory asyncEventHistory)
        {
            return await eventsRepository.AddSyncEventHistory(asyncEventHistory);
        }

        public async Task<SyncEvent> GetAsyncEvent(string id)
        {
            return await eventsRepository.GetSyncEvent(id);
        }

        public async Task<List<SyncEvent>> GetAsyncEvents()
        {
            return await eventsRepository.GetSyncEvents();
        }

        public async Task<List<SyncEvent>> GetProjectAsyncEvent(string projectId)
        {
            var events = await this.eventsRepository.GetSyncEvents();
            var result = events.Where(e => e.ProjectId==projectId).ToList();
            return result;
        }

        public async Task<SyncEventHistory> GetSyncEventHistory(string id)
        {
            return await this.eventsRepository.GetSyncEventHistory(id); 
        }

        public async Task<bool> RemoveAsyncEvent(string id)
        {
            return await eventsRepository.RemoveSyncEvent(id);
        }

        public async Task<SyncEvent> UpdateAsyncEvent(SyncEvent asyncEvent)
        {
            return await eventsRepository.UpdateSyncEvent(asyncEvent);   
        }

        public async Task<SyncEventHistory> UpdateSyncEventHistory(SyncEventHistory asyncEventHistory)
        {
            return await this.eventsRepository.UpdateSyncEventHistory(asyncEventHistory); 
        }
    }
}
