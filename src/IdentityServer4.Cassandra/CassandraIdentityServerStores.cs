using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public class CassandraIdentityServerStores
    {
        private readonly ISession _session;

        public CassandraIdentityServerStores(ISession session)
        {
            _session = session;
        }

        public async Task<IScopeStore> InitializeScopeStoreAsync(params Scope[] scopes)
        {
            var retval = new CassandraScopeStore(_session);
            await retval.InitializeAsync(scopes);
            return retval;
        }

        public async Task<IClientStore> InitializeClientStore(params Client[] clients)
        {
            var retval = new CassandraClientStore(_session);
            await retval.InitializeAsync(clients);
            return retval;
        }

        public async Task<IPersistedGrantStore> InitializeGrantsStoreAsync()
        {
            var retval = new CassandraPersistedGrantStore(_session);
            await retval.InitializeAsync();
            return retval;
        }
    }
}