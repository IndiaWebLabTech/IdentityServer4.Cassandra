using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public class CassandraResourceStore : IResourceStore
    {
        public static CassandraResourceStore Initialize(ISession session)
        {
            var apiStore = CassandraKeyValueStore<string,ApiResource>.Initialize(session, "identityserver_apiresources");
            var identityStore = CassandraKeyValueStore<string,IdentityResource>.Initialize(session, "identityserver_identityresources");
            return new CassandraResourceStore(identityStore, apiStore);
        }
        private readonly IKeyValueStore<string,IdentityResource> _identityStore;
        private readonly IKeyValueStore<string,ApiResource> _apiStore;

        private CassandraResourceStore(CassandraKeyValueStore<string, IdentityResource> identityStore, CassandraKeyValueStore<string, ApiResource> apiStore)
            : this((IKeyValueStore<string, IdentityResource>)identityStore, (IKeyValueStore<string, ApiResource>)apiStore)
        {
        }

        internal CassandraResourceStore(IKeyValueStore<string, IdentityResource> identityStore, IKeyValueStore<string, ApiResource> apiStore)
        {
            _identityStore =identityStore;
            _apiStore = apiStore;
        }

        public Task AddIdentityResourceAsync(IdentityResource resource)
        {
            return _identityStore.SaveAsync(resource.Name, resource);
        }

        public Task AddApiResourceAsync(ApiResource resource)
        {
            return _apiStore.SaveAsync(resource.Name, resource);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var getTasks = new List<Task<IdentityResource>>();
            foreach (var scopeName in scopeNames)
            {
                getTasks.Add(_identityStore.GetAsync(scopeName));
            }
            
            return await Task.WhenAll(getTasks.ToArray());
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var getTasks = new List<Task<ApiResource>>();
            foreach (var scopeName in scopeNames)
            {
                getTasks.Add(_apiStore.GetAsync(scopeName));
            }
            
            return await Task.WhenAll(getTasks.ToArray());
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {           
            return _apiStore.GetAsync(name);
        }

        public async Task<Resources> GetAllResources()
        {
            var identityResourcesTask = _identityStore.ListAsync();
            var apiResourcesTask = _apiStore.ListAsync();
            return new Resources(await identityResourcesTask, await apiResourcesTask);
        }
    }
}


