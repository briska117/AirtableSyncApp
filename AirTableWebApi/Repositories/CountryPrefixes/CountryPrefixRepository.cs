using AirTableDatabase;
using AirTableDatabase.DBModels;
using System;

namespace AirTableWebApi.Repositories.CountryPrefixes
{
    public class CountryPrefixRepository : ICountryPrefixRepository
    {

        /// <summary>The application database</summary>
        private readonly ApplicationDBContext applicationDB;

        /// <summary>Initializes a new instance of the <see cref="CountryPrefixRepository" /> class.</summary>
        /// <param name="applicationDB">The application database.</param>
        public CountryPrefixRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }

        /// <summary>Adds the country prefix.</summary>
        /// <param name="countryPrefix">The country prefix.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.Exception">Error adding country perfix :{ex.Message}</exception>
        public async Task<CountryPrefix> AddCountryPrefix(CountryPrefix countryPrefix)
        {
            try
            {

                CountryPrefix existingCountryPrefix = this.applicationDB.CountryPrefixes.FirstOrDefault(c => c.Name.Trim().ToLower() == countryPrefix.Name.Trim().ToLower());

                if (existingCountryPrefix != null)
                {
                    throw new ArgumentException("Client Prefix Already exist");
                }

                countryPrefix.CountryPrefixId = Guid.NewGuid().ToString();
                await this.applicationDB.CountryPrefixes.AddAsync(countryPrefix);
                await this.applicationDB.SaveChangesAsync();
                return countryPrefix;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding country perfix :{ex.Message}");
            }
            
            
        }

        /// <summary>Gets the country prefix.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.Exception">Error in get country perfix with CountryPrefixId {id} :{ex.Message}</exception>
        public async Task<CountryPrefix> GetCountryPrefix(string id)
        {
            try
            {
                CountryPrefix countryPrefix = this.applicationDB.CountryPrefixes.FirstOrDefault(x => x.CountryPrefixId == id);
                return countryPrefix;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in get country perfix with CountryPrefixId {id} :{ex.Message}");
            }
            
        }

        /// <summary>Gets the country prefixes.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.Exception">Error in get list country perfix :{ ex.Message}</exception>
        public async Task<List<CountryPrefix>> GetCountryPrefixes()
        {
            try {
                List<CountryPrefix> countryPrefixes = this.applicationDB.CountryPrefixes.ToList();
                return countryPrefixes;
            }
            catch(Exception ex) {
                throw new Exception($"Error in get list country perfix :{ ex.Message}");
            }
            
        }

        /// <summary>Removes the country prefix.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.Exception">Error in remove country perfix :{ex.Message}</exception>
        public async Task<bool> RemoveCountryPrefix(string id)
        {
            try
            {
                CountryPrefix countryPrefix = await GetCountryPrefix(id);
                this.applicationDB.CountryPrefixes.Remove(countryPrefix);
                await this.applicationDB.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in remove country perfix :{ex.Message}");
            }
        }



        /// <summary>Updates the country prefix.</summary>
        /// <param name="countryPrefix">The country prefix.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.Exception">Error in update country perfix with CountryPrefixId {countryPrefix.CountryPrefixId} :{ex.Message}</exception>
        public async Task<CountryPrefix> UpdateCountryPrefix(CountryPrefix countryPrefix)
        {
        try
        {
                CountryPrefix dbCountryPrefix =await GetCountryPrefix(countryPrefix.CountryPrefixId);
                applicationDB.Entry(dbCountryPrefix).CurrentValues.SetValues(countryPrefix);
                await applicationDB.SaveChangesAsync();
                return dbCountryPrefix;
        }
        catch (Exception ex)
        {

                throw new Exception($"Error in update country perfix with CountryPrefixId {countryPrefix.CountryPrefixId} :{ex.Message}");
            }
        }
    }
}
