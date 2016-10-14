using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public class CassandraPersistedGrantStore : IPersistedGrantStore
    {
        public static CassandraPersistedGrantStore Initialize(ISession session)
        {
            var kvStore = CassandraKeyValueStore<string,PersistedGrant>.Initialize(session, "identityserver_grants");
            return new CassandraPersistedGrantStore(kvStore);
        }
        private readonly IKeyValueStore<string,PersistedGrant> _store;

        private CassandraPersistedGrantStore(CassandraKeyValueStore<string, PersistedGrant> store)
        {
            _store = store;
        }

        internal CassandraPersistedGrantStore(IKeyValueStore<string, PersistedGrant> store)
        {
            _store = store;
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            return _store.SaveAsync(grant.Key, grant);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            return _store.GetAsync(key);
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var dtos = await _store.ListAsync();
            return dtos.Where(s => s.SubjectId == subjectId);
        }

        public Task RemoveAsync(string key)
        {
            return _store.RemoveAsync(key);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var dtos = await GetAllAsync(subjectId);
            var removalTasks = new List<Task>();
            foreach (var dto in dtos.Where(d => d.SubjectId == subjectId && d.ClientId == clientId).ToArray())
            {
                removalTasks.Add(RemoveAsync(dto.Key));
            }
            await Task.WhenAll(removalTasks);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var dtos = await GetAllAsync(subjectId);
            var removalTasks = new List<Task>();
            foreach (var dto in dtos.Where(d => d.SubjectId == subjectId && d.ClientId == clientId && d.Type == type).ToArray())
            {
                removalTasks.Add(RemoveAsync(dto.Key));
            }
            await Task.WhenAll(removalTasks);
        }
    }
}