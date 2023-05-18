using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.CountryPrefixes
{
    public interface ICountryPrefixRepository
    {
        public Task<List<CountryPrefix>> GetCountryPrefixes();
        public Task<CountryPrefix> GetCountryPrefix(string id);
        public Task<CountryPrefix> AddCountryPrefix(CountryPrefix countryPrefix);
        public Task<CountryPrefix> UpdateCountryPrefix(CountryPrefix countryPrefix);
        public Task<bool> RemoveCountryPrefix(string id);  
    }
}
