using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.SyncEvents;

namespace AirTableWebApi.Services.SyncEvents
{
    public class SyncEventsService : ISyncEventsService
    {
        private readonly ISyncEventsRepository eventsRepository;

        public SyncEventsService(ISyncEventsRepository eventsRepository)
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

        public async Task<SyncEvent> GetSyncEvent(string id)
        {
            return await eventsRepository.GetSyncEvent(id);
        }

        public async Task<List<SyncEvent>> GetSyncEvents()
        {
            return await eventsRepository.GetSyncEvents();
        }

        public async Task<List<SyncEventHistory>> GetEventHistoryByEventtId(string eventId)
        {
            return await this.eventsRepository.GetEventHistoryByEventId(eventId);
        }

        public async Task<List<SyncEvent>> GetProjectAsyncEvent(string projectId)
        {
            var events = await this.eventsRepository.GetSyncEvents();
            var result = events.Where(e => e.ProjectId==projectId).ToList();
            return result;
        }

        public async Task<List<SyncEventsView>> GetProjectSyncEventFull(string projectId)
        {
            List<SyncEventsView> eventsViews = new List<SyncEventsView>();
            var events = await this.GetSyncEventsByProject(projectId);
            foreach (var item in events)
            {
                eventsViews.Add(
                    new SyncEventsView {
                        SyncEvent = item,
                        SyncEventHistories = await this.GetEventHistoryByEventtId(item.SyncEventId)
                    });
            }

            return eventsViews;
        }

        public async Task<SyncEventHistory> GetSyncEventHistory(string id)
        {
            return await this.eventsRepository.GetSyncEventHistory(id); 
        }

        public Task<List<SyncEvent>> GetSyncEventsByProject(string projectId)
        {
            return this.eventsRepository.GetSyncEventsByProject(projectId); 
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
