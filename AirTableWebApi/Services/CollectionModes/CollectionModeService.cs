using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.CollectionModes;
using AirTableWebApi.Repositories.CountryPrefixes;

namespace AirTableWebApi.Services.CollectionModes
{
    public class CollectionModeService : ICollectionModeService
    {
        private readonly ICollectionModeRepository collectionModeRepository;

        public CollectionModeService(ICollectionModeRepository collectionModeRepository)
        {
            this.collectionModeRepository = collectionModeRepository;
        }
        public async Task<CollectionMode> AddCollectionMode(CollectionMode collectionMode)
        {
            return await collectionModeRepository.AddCollectionMode(collectionMode);
        }

        public async Task<CollectionModeRelatedTable> AddCollectionModeRelatedTable(CollectionModeRelatedTable modeRelatedTable)
        {
            return await collectionModeRepository.AddCollectionModeRelatedTable(modeRelatedTable);    
        }

        public async Task<bool> DeleteCollectionMode(string id)
        {
            return await collectionModeRepository.DeleteCollectionMode(id);   
        }

        public async Task<bool> DeleteCollectionModeRelatedTable(string collectionId, string relatedTableId)
        {
            return await collectionModeRepository.DeleteCollectionModeRelatedTable(collectionId,relatedTableId);
        }

        public async Task<bool> DeleteRangeCollectionModeRelatedTable(List<CollectionModeRelatedTable> relatedTables)
        {
            return await collectionModeRepository.DeleteRangeCollectionModeRelatedTable(relatedTables);
        }

        public async Task<CollectionMode> GetCollectionMode(string id)
        {
            return await collectionModeRepository.GetCollectionMode(id);
        }

        public Task<List<CollectionModeRelatedTable>> GetCollectionModeRelatedTable(string CollectionModeId)
        {
            return collectionModeRepository.GetCollectionModeRelatedTable(CollectionModeId);    
        }

        public async Task<List<CollectionMode>> GetCollectionModes()
        {
            return await collectionModeRepository.GetCollectionModes();
        }

        public async Task<CollectionMode> UpdateCollectionMode(CollectionMode collectionMode)
        {
            return await collectionModeRepository.UpdateCollectionMode(collectionMode);   
        }
    }
}
