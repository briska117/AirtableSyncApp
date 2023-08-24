using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.CollectionModes
{
    public interface ICollectionModeRepository
    {

        public Task<List<CollectionMode>> GetCollectionModes();
        public Task<CollectionMode> GetCollectionMode(string id);
        public Task<CollectionMode> AddCollectionMode(CollectionMode collectionMode);
        public Task<CollectionMode> UpdateCollectionMode(CollectionMode collectionMode);
        public Task<bool> DeleteCollectionMode(string id);
        public Task<CollectionModeRelatedTable> AddCollectionModeRelatedTable(CollectionModeRelatedTable modeRelatedTable);
        public Task<List<CollectionModeRelatedTable>> GetCollectionModeRelatedTable(string CollectionModeId);
        public Task<bool> DeleteCollectionModeRelatedTable(string collectionId, string relatedTableId);
        public Task<bool> DeleteRangeCollectionModeRelatedTable(List<CollectionModeRelatedTable> relatedTables);


    }
}
