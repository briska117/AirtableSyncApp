using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.AirTableFields;

namespace AirTableWebApi.Services.AirTableFields
{
    public class AirTableFieldsService : IAirTableFieldsService
    {
        private readonly IAirTableFieldsRepository airTableFields;

        public AirTableFieldsService(IAirTableFieldsRepository airTableFields)
        {
            this.airTableFields = airTableFields;
        }
        public async Task<AirTableField> AddAirTableField(AirTableField airTableField)
        {
            return await airTableFields.AddAirTableField(airTableField);  
        }

        public Task<List<AirTableField>> AddAirTableFields(List<AirTableField> airTableFields)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AirTableField>> GetAirTableFieldsByTable(string tableId)
        {
            return await airTableFields.GetAirTableFieldsByTable(tableId);     
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
