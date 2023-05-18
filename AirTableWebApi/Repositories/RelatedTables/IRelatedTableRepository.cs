using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.RelatedTables
{
    public interface IRelatedTableRepository
    {
        public Task<List<RelatedTable>> GetRelatedTables();
        public Task<RelatedTable> GetRelatedTable(string id);
        public Task<RelatedTable> AddRelatedTable(RelatedTable relatedTable);
        public Task<RelatedTable> UpdateRelatedTable(RelatedTable relatedTable);
        public Task<bool> RemoveRelatedTable(string id);

    }
}
