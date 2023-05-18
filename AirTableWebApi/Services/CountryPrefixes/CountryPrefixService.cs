using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.CountryPrefixes;

namespace AirTableWebApi.Services.CountryPrefixes
{
    public class CountryPrefixService : ICountryPrefixService
    {
        private readonly ICountryPrefixRepository countryPrefix;

        public CountryPrefixService(ICountryPrefixRepository countryPrefix)
        {
            this.countryPrefix = countryPrefix;
        }
        public async Task<CountryPrefix> AddCountryPrefix(CountryPrefix countryPrefix)
        {
            return await this.countryPrefix.AddCountryPrefix(countryPrefix);    
        }

        public async Task<bool> ExistCountryPrefix(string id)
        {
            var exist =await this.countryPrefix.GetCountryPrefix(id);
            if(exist == null)
            {
                return false;
            }

            return true;
        }

        public async Task<CountryPrefix> GetCountryPrefix(string id)
        {
            return await this.countryPrefix.GetCountryPrefix(id);
        }

        public async Task<List<CountryPrefix>> GetCountryPrefixes()
        {
            return await this.countryPrefix.GetCountryPrefixes();
        }

        public async Task<bool> RemoveCountryPrefix(string id)
        {
            return await this.countryPrefix.RemoveCountryPrefix(id);  
        }

        public async Task<CountryPrefix> UpdateCountryPrefix(CountryPrefix countryPrefix)
        {
            return await this.countryPrefix.UpdateCountryPrefix(countryPrefix);
        }
    }
}
