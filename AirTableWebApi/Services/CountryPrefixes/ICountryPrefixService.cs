using AirTableDatabase.DBModels;

namespace AirTableWebApi.Services.CountryPrefixes
{
    public interface ICountryPrefixService
    {

        public Task<List<CountryPrefix>> GetCountryPrefixes();
        public Task<bool> ExistCountryPrefix(string id);
        public Task<CountryPrefix> GetCountryPrefix(string id);
        public Task<CountryPrefix> AddCountryPrefix(CountryPrefix countryPrefix);
        public Task<CountryPrefix> UpdateCountryPrefix(CountryPrefix countryPrefix);
        public Task<bool> RemoveCountryPrefix(string id);
    }
}
