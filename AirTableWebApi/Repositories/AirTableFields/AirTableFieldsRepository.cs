using AirTableDatabase;
using AirTableDatabase.DBModels;
using Microsoft.EntityFrameworkCore;

namespace AirTableWebApi.Repositories.AirTableFields
{
    public class AirTableFieldsRepository : IAirTableFieldsRepository
    {
        private readonly ApplicationDBContext applicationDB;

        public AirTableFieldsRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }
        public async Task<AirTableField> AddAirTableField(AirTableField airTableField)
        {
            try
            {
                airTableField.AirTableFieldId = Guid.NewGuid().ToString();
                await applicationDB.AirTableFields.AddAsync(airTableField);
                await applicationDB.SaveChangesAsync();
                return airTableField;   
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task<List<AirTableField>> AddAirTableFields(List<AirTableField> airTableFields)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AirTableField>> GetAirTableFieldsByTable(string tableId)
        {
            try
            {
                return await this.applicationDB.AirTableFields.Where(a => a.RelatedTableId == tableId).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> RemoveAirTableField(string airtableFieldId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAirTableFieldsByTable(string tableId)
        {
            throw new NotImplementedException();
        }
    }
}
