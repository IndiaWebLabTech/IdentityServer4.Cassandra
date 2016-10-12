using System.Collections.Generic;
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
            var store = CassandraScopeStore.Initialize(session);
            var saveTasks = new List<Task>();
            foreach(var scope in scopes)
            {
                saveTasks.Add(store.AddScopeAsync(scope));
            }
            await Task.WhenAll(saveTasks);
            return store;
        }

        public static async Task<IClientStore> InitializeClientStore(ISession session, params Client[] clients)
        {
            var store = CassandraClientStore.Initialize(session);
            var saveTasks = new List<Task>();
            foreach(var scope in clients)
            {
                saveTasks.Add(store.AddClient(scope));
            }
            await Task.WhenAll(saveTasks);
            return store;
        }

        public static Task<IPersistedGrantStore> InitializeGrantsStoreAsync(ISession session)
        {
            return Task.FromResult((IPersistedGrantStore)CassandraPersistedGrantStore.Initialize(session));
        }
    }
}