using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    internal class CassandraScopeStore : IScopeStore
    {
        public static CassandraScopeStore Initialize(ISession session)
        {
            var kvStore = CassandraKeyValueStore<string,Scope>.Initialize(session, "identityserver_grants");
            return new CassandraScopeStore(kvStore);
        }
        private readonly IKeyValueStore<string,Scope> _store;

        private CassandraScopeStore(CassandraKeyValueStore<string, Scope> store)
        {
            _store = store;
        }

        internal CassandraScopeStore(IKeyValueStore<string, Scope> store)
        {
            _store = store;
        }

        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var getTasks = new List<Task<Scope>>();
            foreach (var scopeName in scopeNames)
            {
                getTasks.Add(_store.GetAsync(scopeName));
            }
            
            var customScopes = await Task.WhenAll(getTasks.ToArray());
            return customScopes.Union(StandardScopes.All.Where(s => scopeNames.Contains(s.Name)));
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var all = await _store.ListAsync();
            return publicOnly ? StandardScopes.All.Union(all).Where(s => s.ShowInDiscoveryDocument) : all;
        }

        public Task AddScopeAsync(Scope scope)
        {
            return _store.SaveAsync(scope.Name, scope);
        }
    }
}


