using AirTableDatabase;
using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.ClientPrefixes
{
    public class ClientPrefixRepository : IClientPrefixRepository
    {
        private readonly ApplicationDBContext applicationDB;

        public ClientPrefixRepository(ApplicationDBContext applicationDB)
        {
            this.applicationDB = applicationDB;
        }
        public async Task<ClientPrefix> AddClientPrefix(ClientPrefix clientPrefix)
        {
            try
            {
                clientPrefix.ClientPrefixId = Guid.NewGuid().ToString();
                await this.applicationDB.ClientPrefixes.AddAsync(clientPrefix);
                await this.applicationDB.SaveChangesAsync();
                return clientPrefix;    
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in Add Client Prefix : {ex.Message}");
            }
        }

        public async Task<ClientPrefix> GetClientPrefix(string id)
        {
            try
            {
                ClientPrefix clientPrefix = this.applicationDB.ClientPrefixes.FirstOrDefault(cp => cp.ClientPrefixId == id);
                return clientPrefix;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in get Client Prefix with Id {id} : {ex.Message}");
            }
        }

        public async Task<List<ClientPrefix>> GetClientPrefixes()
        {
            try
            {
                List<ClientPrefix> clientPrefixes = this.applicationDB.ClientPrefixes.ToList();
                return clientPrefixes;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in get List of Client Prefixes : {ex.Message}");
            }
        }

        public async Task<bool> RemovePrefix(string id)
        {
            try
            {
                ClientPrefix clientPrefix = await GetClientPrefix(id);
                this.applicationDB.ClientPrefixes.Remove(clientPrefix);
                await this.applicationDB.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in remove Client Prefix with Id {id} : {ex.Message}");
            }
        }

        public async Task<ClientPrefix> UpdateClientPrefix(ClientPrefix clientPrefix)
        {
            try
            {
                ClientPrefix clientPrefixDB = await GetClientPrefix(clientPrefix.ClientPrefixId);
                clientPrefixDB.Name=clientPrefix.Name;
                this.applicationDB.ClientPrefixes.Entry(clientPrefixDB);
                await this.applicationDB.SaveChangesAsync();
                return clientPrefixDB;  
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in update Client Prefix with Id {clientPrefix.ClientPrefixId} : {ex.Message}");
            }
            
        }
    }
}
