using AirTableDatabase;
using AirTableDatabase.DBModels;

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

        public Task<List<AirTableField>> GetAirTableFieldsByTable(string tableId)
        {
            throw new NotImplementedException();
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
