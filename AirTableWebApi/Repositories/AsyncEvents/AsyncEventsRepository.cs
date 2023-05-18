﻿using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.AsyncEvents
{
    public class AsyncEventsRepository : IAsyncEventsRepository
    {
        private readonly ApplicationDBContext applicationDB;

        public AsyncEventsRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }
        public async Task<SyncEvent> AddAsyncEvent(SyncEvent asyncEvent)
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

        public async Task<SyncEvent> GetAsyncEvent(string id)
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

        public async Task<List<SyncEvent>> GetAsyncEvents()
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

        public async Task<bool> RemoveAsyncEvent(string id)
        {
            try
            {
                SyncEvent AsyncEventDB =await GetAsyncEvent(id);
                this.applicationDB.SyncEvents.Remove(AsyncEventDB);
                await this.applicationDB.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SyncEvent> UpdateAsyncEvent(SyncEvent asyncEvent)
        {
            try
            {
                SyncEvent AsyncEventDB = await GetAsyncEvent(asyncEvent.SyncEventId);
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
    }
}
