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
    internal class CassandraPersistedGrantStore : IPersistedGrantStore
    {
        private const string TABLE_NAME = "IdentityServer4_Grants";
        private const string SCHEMA_INITIALIZATION = @"
CREATE TABLE IF NOT EXISTS {0}(key text,subjectid text,clientid text,type text, data text, PRIMARY KEY (key));";

        static CassandraPersistedGrantStore()
        {
            MappingConfiguration.Global.Define(new Map<PersistedGrantDto>()
                .TableName(TABLE_NAME)
                .PartitionKey(s => s.Key));
        }

        private readonly ISession _session;

        public CassandraPersistedGrantStore(ISession session)
        {
            _session = session;
        }

        internal async Task InitializeAsync()
        {

            var createSchemaCql = String.Format(SCHEMA_INITIALIZATION, TABLE_NAME);
            await _session.ExecuteAsync(_session.Prepare(createSchemaCql).Bind());
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            var mapper = new Mapper(_session);
            await mapper.InsertAsync(PersistedGrantDto.FromPersistedGrant(grant));
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var mapper = new Mapper(_session);
            var dto = await mapper.FirstOrDefaultAsync<PersistedGrantDto>("where key = ?", key);
            return dto.ToPersistedGrantDto();
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var mapper = new Mapper(_session);
            var dtos = await mapper.FetchAsync<PersistedGrantDto>();
            return dtos.Where(s => s.SubjectId == subjectId).Select(d => d.ToPersistedGrantDto());
        }

        public async Task RemoveAsync(string key)
        {
            var mapper = new Mapper(_session);
            await mapper.DeleteAsync<PersistedGrantDto>("where key = ?", key);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var mapper = new Mapper(_session);
            var dtos = await mapper.FetchAsync<PersistedGrantDto>();
            var removalTasks = new List<Task>();
            foreach (var dto in dtos.Where(d => d.SubjectId == subjectId && d.ClientId == clientId))
            {
                removalTasks.Add(RemoveAsync(dto.Key));
            }
            await Task.WhenAll(removalTasks);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var mapper = new Mapper(_session);
            var dtos = await mapper.FetchAsync<PersistedGrantDto>();
            var removalTasks = new List<Task>();
            foreach (var dto in dtos.Where(d => d.SubjectId == subjectId && d.ClientId == clientId && d.Type == type))
            {
                removalTasks.Add(RemoveAsync(dto.Key));
            }
            await Task.WhenAll(removalTasks);
        }

        class PersistedGrantDto
        {

            public PersistedGrantDto()
            {
            }

            public PersistedGrantDto(string key, string subjectId, string clientId, string type, string data)
            {
                Key = key;
                SubjectId = subjectId;
                ClientId = clientId;
                Type = type;
                Data = data;
            }

            public static PersistedGrantDto FromPersistedGrant(PersistedGrant grant)
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(grant);
                return new PersistedGrantDto(grant.Key, grant.SubjectId, grant.ClientId, grant.Type, jsonData);
            }

            public PersistedGrant ToPersistedGrantDto()
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<PersistedGrant>(this.Data);
            }

            public string Key { get; set; }
            public string SubjectId { get; set; }
            public string ClientId { get; set; }
            public string Type { get; set; }
            public string Data { get; set; }
        }
    }
}