using AirTableDatabase.DBModels;
using AirTableWebApi.Repositories.ClientPrefixes;

namespace AirTableWebApi.Services.ClientPrefixes
{
    public class ClientPrefixService : IClientPrefixService
    {
        private readonly IClientPrefixRepository prefixRepository;

        public ClientPrefixService(IClientPrefixRepository prefixRepository)
        {
            this.prefixRepository = prefixRepository;
        }

        public async Task<ClientPrefix> AddClientPrefix(ClientPrefix clientPrefix)
        {
            await this.prefixRepository.AddClientPrefix(clientPrefix);
            return clientPrefix;    
        }

        public async Task<bool> DeleteClientPrefix(string id)
        {
            bool success = await this.prefixRepository.RemovePrefix(id);
            return success;
        }

        public async Task<bool> ExistClientPrefix(string id)
        {
            var exist = await this.prefixRepository.GetClientPrefix(id);
            if(exist == null)
            {
                return false;
            }
            return true;
        }

        public async Task<ClientPrefix> GetClientPrefix(string id)
        {
            return await this.prefixRepository.GetClientPrefix(id);
        }

        public Task<List<ClientPrefix>> GetClientPrefixes()
        {
            return this.prefixRepository.GetClientPrefixes();
        }

        public async Task<ClientPrefix> UpdateClientPrefix(ClientPrefix clientPrefix)
        {
            return await this.prefixRepository.UpdateClientPrefix(clientPrefix);
        }
    }
}
