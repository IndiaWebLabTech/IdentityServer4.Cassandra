using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public static class CassandraIdentityServerStores
    {

        public static async Task<IScopeStore> InitializeScopeStoreAsync(ISession session, params Scope[] scopes)
        {
            var retval = new CassandraScopeStore(session);
            await retval.InitializeAsync(scopes);
            return retval;
        }

        public static async Task<IClientStore> InitializeClientStore(ISession session, params Client[] clients)
        {
            var retval = new CassandraClientStore(session);
            await retval.InitializeAsync(clients);
            return retval;
        }

        public static async Task<IPersistedGrantStore> InitializeGrantsStoreAsync(ISession session)
        {
            var retval = new CassandraPersistedGrantStore(session);
            await retval.InitializeAsync();
            return retval;
        }
    }
}