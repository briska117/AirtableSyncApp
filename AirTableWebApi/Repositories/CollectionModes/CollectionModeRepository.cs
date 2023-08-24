using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.CollectionModes
{
    public class CollectionModeRepository : ICollectionModeRepository
    {
        private readonly ApplicationDBContext applicationDB;

        public CollectionModeRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }
        public async Task<CollectionMode> AddCollectionMode(CollectionMode collectionMode)
        {
            try
            {
                collectionMode.CollectionModeId = Guid.NewGuid().ToString();
                this.applicationDB.CollectionModes.Add(collectionMode);
                this.applicationDB.SaveChanges(); 
                return collectionMode;  

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Add Collection Mode : {ex.Message}");
            }
            
        }

        public async Task<CollectionModeRelatedTable> AddCollectionModeRelatedTable(CollectionModeRelatedTable modeRelatedTable)
        {
            try
            {
                modeRelatedTable.CollectionModeRelatedTableId = Guid.NewGuid().ToString();
                await this.applicationDB.CollectionModeRelatedTables.AddAsync(modeRelatedTable);
                this.applicationDB.SaveChanges();
                return modeRelatedTable;

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Add Collection Mode : {ex.Message}");
            }
        }

        public async Task<bool> DeleteCollectionMode(string id)
        {
            try
            {
                CollectionMode collectionMode =await this.GetCollectionMode(id);
                this.applicationDB.CollectionModes.Remove(collectionMode);
                await this.applicationDB.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in delete Collection Mode with Id {id}: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCollectionModeRelatedTable(string collectionId, string relatedTableId)
        {
            try
            {
                List<CollectionModeRelatedTable> collectionModes = await this.GetCollectionModeRelatedTable(collectionId);
                CollectionModeRelatedTable table = collectionModes.Where(r => r.RelatedTableId == relatedTableId).FirstOrDefault();
                this.applicationDB.CollectionModeRelatedTables.Remove(table);
                await this.applicationDB.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in delete Collection Mode with Id {collectionId}: {ex.Message}");
            }
        }

        public async Task<bool> DeleteRangeCollectionModeRelatedTable(List<CollectionModeRelatedTable> relatedTables)
        {
            try
            {
                this.applicationDB.CollectionModeRelatedTables.RemoveRange(relatedTables);
                await this.applicationDB.SaveChangesAsync();    
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<CollectionMode> GetCollectionMode(string id)
        {
            try
            {
                CollectionMode collectionMode = this.applicationDB.CollectionModes.FirstOrDefault(cm => cm.CollectionModeId.Equals(id)); ;
                return collectionMode;

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Get Collection Mode with Id {id}: {ex.Message}");
            }
        }

        public async Task<List<CollectionModeRelatedTable>> GetCollectionModeRelatedTable(string CollectionModeId)
        {
            try
            {
                return await this.applicationDB.CollectionModeRelatedTables.Where(cr => cr.CollectionModeId == CollectionModeId).ToListAsync();

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Get Collection Mode with Id {CollectionModeId}: {ex.Message}");
            }
        }

        public async Task<List<CollectionMode>> GetCollectionModes()
        {
            try
            {
                List<CollectionMode> collectionModes = this.applicationDB.CollectionModes.ToList(); ;    
                return collectionModes;

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Get List of Collection Mode : {ex.Message}");
            }
        }

        public async Task<CollectionMode> UpdateCollectionMode(CollectionMode collectionMode)
        {

            try
            {
                CollectionMode collectionModeDB = await this.GetCollectionMode(collectionMode.CollectionModeId);
                this.applicationDB.CollectionModes.Entry(collectionModeDB).CurrentValues.SetValues(collectionMode);
                await this.applicationDB.SaveChangesAsync();    
                return collectionMode;

            }
            catch (Exception ex)
            {

                throw new Exception($"Error in update Collection Mode whith Id {collectionMode.CollectionModeId}: {ex.Message}");
            }
        }
    }
}
