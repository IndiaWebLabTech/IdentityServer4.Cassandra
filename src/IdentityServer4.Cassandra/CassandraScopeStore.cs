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
        private const string TABLE_NAME = "IdentityServer4_Scopes";
        private const string SCHEMA_INITIALIZATION = @"
CREATE TABLE IF NOT EXISTS {0}(name text PRIMARY KEY, data text);";

        static CassandraScopeStore()
        {
            MappingConfiguration.Global.Define(new Map<ScopeDto>()
                .TableName(TABLE_NAME)
                .PartitionKey(s => s.Name));
        }

        private readonly ISession _session;

        public CassandraScopeStore(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            var mapper = new Mapper(_session);
            var getTasks = new List<Task<ScopeDto>>();
            foreach (var scopeName in scopeNames)
            {
                getTasks.Add(mapper.FirstOrDefaultAsync<ScopeDto>("where name = ?", scopeName));
            }

            var completed =  await Task.WhenAll(getTasks.ToArray());
            return completed.Select(c => c.ToScope());
        }

        public async Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            var mapper = new Mapper(_session);
            var all = await mapper.FetchAsync<ScopeDto>();
            var scopes = all.Select(c => c.ToScope());
            return publicOnly ? scopes.Where(s => s.ShowInDiscoveryDocument) : scopes;
        }

        internal async Task InitializeAsync(params Scope[] scopes)
        {
            var createSchemaCql = String.Format(SCHEMA_INITIALIZATION, TABLE_NAME);
            await _session.ExecuteAsync(_session.Prepare(createSchemaCql).Bind());
            var mapper = new Mapper(_session);
            var insertTasks = new List<Task>();

            foreach (var scope in scopes)
            {
                insertTasks.Add(mapper.InsertAsync(ScopeDto.FromScope(scope)));
            }

            await Task.WhenAll(insertTasks.ToArray());
        }

        class ScopeDto
        {
            public ScopeDto()
            {
            }

            public ScopeDto(string name, string data)
            {
                Name = name;
                Data = data;
            }

            public static ScopeDto FromScope(Scope scope)
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(scope);
                return new ScopeDto(scope.Name, jsonData);
            }

            public Scope ToScope()
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Scope>(this.Data);
            }

            public string Name { get; set; }
            public string Data { get; set; }
        }
    }
}


