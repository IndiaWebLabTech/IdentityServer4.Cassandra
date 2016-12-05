using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public class CassandraClientStore : IClientStore
    {
        public static CassandraClientStore Initialize(ISession session)
        {
            var kvStore = CassandraKeyValueStore<string,Client>.Initialize(session, "identityserver_clients");
            return new CassandraClientStore(kvStore);
        }

        private readonly IKeyValueStore<string,Client> _store;

        private CassandraClientStore(CassandraKeyValueStore<string,Client> store)
        {
            _store = store;
        }

        internal CassandraClientStore(IKeyValueStore<string,Client> store)
        {
            _store = store;
        }

        public Task AddClient(Client client)
        {
            return _store.SaveAsync(client.ClientId, client);
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return _store.GetAsync(clientId);
        }
    }
}