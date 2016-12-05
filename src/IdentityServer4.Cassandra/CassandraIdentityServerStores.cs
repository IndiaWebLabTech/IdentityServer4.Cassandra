using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public static class CassandraIdentityServerStores
    {
        
        public static async Task<IResourceStore> InitializeScopeStoreAsync(ISession session,  
            IEnumerable<ApiResource> apiResources = null,
            IEnumerable<IdentityResource> identityResource = null)
        {
            var store = CassandraResourceStore.Initialize(session);
            var saveTasks = new List<Task>();

            foreach(var scope in apiResources ?? Enumerable.Empty<ApiResource>())
            {
                saveTasks.Add(store.AddApiResourceAsync(scope));
            }

            foreach(var scope in identityResource ?? Enumerable.Empty<IdentityResource>())
            {
                saveTasks.Add(store.AddIdentityResourceAsync(scope));
            }

            await Task.WhenAll(saveTasks).ConfigureAwait(false);
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
            await Task.WhenAll(saveTasks).ConfigureAwait(false);
            return store;
        }

        public static Task<IPersistedGrantStore> InitializeGrantsStoreAsync(ISession session)
        {
            return Task.FromResult((IPersistedGrantStore)CassandraPersistedGrantStore.Initialize(session));
        }
    }
}