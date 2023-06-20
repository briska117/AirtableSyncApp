using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.SyncEvents
{
    public class SyncEventsRepository : ISyncEventsRepository
    {
        private readonly ApplicationDBContext applicationDB;
        public SyncEventsRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }
        public async Task<SyncEvent> AddSyncEvent(SyncEvent asyncEvent)
        {
            try {
                asyncEvent.SyncEventId = Guid.NewGuid().ToString();
                await this.applicationDB.SyncEvents.AddAsync(asyncEvent);
                await this.applicationDB.SaveChangesAsync();
                return asyncEvent;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SyncEventHistory> AddSyncEventHistory(SyncEventHistory asyncEventHistory)
        {
            try
            {
                asyncEventHistory.SyncEventHistoryId = Guid.NewGuid().ToString();
                await this.applicationDB.EventHistories.AddAsync(asyncEventHistory);
                await this.applicationDB.SaveChangesAsync();
                return asyncEventHistory;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SyncEvent> GetSyncEvent(string id)
        {
            try
            {
                SyncEvent asyncEvent = await this.applicationDB.SyncEvents.FirstOrDefaultAsync(ae => ae.SyncEventId == id);
                return asyncEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<SyncEvent>> GetSyncEvents()
        {
            try
            {
                return await this.applicationDB.SyncEvents.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SyncEventHistory> GetSyncEventHistory(string id)
        {
            try
            {
                return await this.applicationDB.EventHistories.FirstOrDefaultAsync(h=> h.SyncEventHistoryId == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> RemoveSyncEvent(string id)
        {
            try
            {
                SyncEvent AsyncEventDB =await GetSyncEvent(id);
                this.applicationDB.SyncEvents.Remove(AsyncEventDB);
                await this.applicationDB.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SyncEvent> UpdateSyncEvent(SyncEvent asyncEvent)
        {
            try
            {
                SyncEvent AsyncEventDB = await GetSyncEvent(asyncEvent.SyncEventId);
                this.applicationDB.SyncEvents.Entry(AsyncEventDB).CurrentValues.SetValues(asyncEvent);
                await this.applicationDB.SaveChangesAsync();
                return asyncEvent;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<SyncEventHistory> UpdateSyncEventHistory(SyncEventHistory asyncEventHistory)
        {
            try
            {
                SyncEventHistory asyncEventHistoryDB = await GetSyncEventHistory(asyncEventHistory.SyncEventHistoryId);
                this.applicationDB.EventHistories.Entry(asyncEventHistoryDB).CurrentValues.SetValues(asyncEventHistory);
                await this.applicationDB.SaveChangesAsync();
                return asyncEventHistory;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<SyncEvent>> GetSyncEventsByProject(string projectId)
        {
            try
            {
                return await this.applicationDB.SyncEvents.Where(e => e.ProjectId == projectId).OrderByDescending(e=>e.SyncTime).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<SyncEventHistory>> GetEventHistoryByEventId(string eventId)
        {
            try
            {
                return await this.applicationDB.EventHistories.Where(e => e.SyncEventId == eventId).OrderBy(e=>e.StartSync).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
