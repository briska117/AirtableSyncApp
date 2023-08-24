using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.AirTableFields
{
    public interface IAirTableFieldsService
    {
        public Task<List<AirTableField>> GetAirTableFieldsByTable(string tableId);
        public Task<AirTableField> AddAirTableField(AirTableField airTableField);
        public Task<List<AirTableField>> AddAirTableFields(List<AirTableField> airTableFields);
        public Task<bool> RemoveAirTableField(string airtableFieldId);
        public Task<bool> RemoveAirTableFieldsByTable(string tableId);
    }
}
