using AirTableDatabase.DBModels;

namespace AirTableWebApi.Repositories.ClientPrefixes
{
    public interface IClientPrefixRepository
    {
        public Task<ClientPrefix> GetClientPrefix(string id);
        public Task<List<ClientPrefix>> GetClientPrefixes();
        public Task<ClientPrefix> AddClientPrefix(ClientPrefix clientPrefix);
        public Task<ClientPrefix> UpdateClientPrefix(ClientPrefix clientPrefix);
        public Task<bool> RemovePrefix(string id);
    }
}
