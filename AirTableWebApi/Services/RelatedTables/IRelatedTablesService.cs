using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.RelatedTables
{
    public interface IRelatedTablesService
    {
        public Task<List<RelatedTable>> GetRelatedTables();
        public Task<RelatedTable> GetRelatedTable(string id);
        public Task<RelatedTable> AddRelatedTable(RelatedTable relatedTable);
        public Task<RelatedTable> UpdateRelatedTable(RelatedTable relatedTable);
        public Task<bool> RemoveRelatedTable(string id);
    }
}
