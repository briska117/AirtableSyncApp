using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.RelatedTables;

namespace AirTableWebApi.Services.RelatedTables
{
    public class RelatedTablesService : IRelatedTablesService
    {
        private readonly IRelatedTableRepository relatedTableRepository;

        public RelatedTablesService(IRelatedTableRepository relatedTableRepository)
        {
            this.relatedTableRepository = relatedTableRepository;
        }
        public async Task<RelatedTable> AddRelatedTable(RelatedTable relatedTable)
        {
            return await this.relatedTableRepository.AddRelatedTable(relatedTable);
        }

        public async Task<RelatedTable> GetRelatedTable(string id)
        {
            return await this.relatedTableRepository.GetRelatedTable(id);
        }

        public async Task<List<RelatedTable>> GetRelatedTables()
        {
            return await this.relatedTableRepository.GetRelatedTables(); 
        }

        public async Task<bool> RemoveRelatedTable(string id)
        {
            return await this.relatedTableRepository.RemoveRelatedTable(id);  
        }

        public async Task<RelatedTable> UpdateRelatedTable(RelatedTable relatedTable)
        {
            return await this.relatedTableRepository.UpdateRelatedTable(relatedTable);
        }
    }
}
