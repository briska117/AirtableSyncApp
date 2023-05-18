using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.RelatedTables
{
    public class RelatedTableRepository: IRelatedTableRepository
    {
        private readonly ApplicationDBContext applicationDB;

        public RelatedTableRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }

        public async Task<RelatedTable> AddRelatedTable(RelatedTable relatedTable)
        {
            try
            {
                relatedTable.RelatedTableId = Guid.NewGuid().ToString();
                await this.applicationDB.RelatedTables.AddAsync(relatedTable);
                await this.applicationDB.SaveChangesAsync();
                return relatedTable;    
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<RelatedTable> GetRelatedTable(string id)
        {
            try
            {
                return await this.applicationDB.RelatedTables.FirstOrDefaultAsync(r => r.RelatedTableId == id);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<RelatedTable>> GetRelatedTables()
        {
            try
            {
                return await this.applicationDB.RelatedTables.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RemoveRelatedTable(string id)
        {
            try
            {
                RelatedTable relatedTableDb = await this.GetRelatedTable(id);
                this.applicationDB.RelatedTables.Remove(relatedTableDb);
                await this.applicationDB.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<RelatedTable> UpdateRelatedTable(RelatedTable relatedTable)
        {
            try
            {
                RelatedTable relatedTableDb = await this.GetRelatedTable(relatedTable.RelatedTableId);
                this.applicationDB.RelatedTables.Entry(relatedTableDb).CurrentValues.SetValues(relatedTable);
                await this.applicationDB.SaveChangesAsync();
                return relatedTable;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
