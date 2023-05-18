using AirTableDatabase.DBModels;
namespace AirTableWebApi.Services.ClientPrefixes
{
    public interface IClientPrefixService
    {
        public Task<bool> ExistClientPrefix(string id);
        public Task<ClientPrefix> GetClientPrefix(string id);
        public Task<List<ClientPrefix>> GetClientPrefixes();
        public Task<ClientPrefix> AddClientPrefix(ClientPrefix clientPrefix);
        public Task<ClientPrefix> UpdateClientPrefix(ClientPrefix clientPrefix);
        public Task<bool> DeleteClientPrefix(string id);

    }
}
