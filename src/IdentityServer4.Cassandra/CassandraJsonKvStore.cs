using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;

namespace IdentityServer4.Cassandra
{
    public class CassandraKeyValueStore<TKey, TData> : IKeyValueStore<TKey, TData> where TData : class
    {

        public static CassandraKeyValueStore<TKey, TData> Initialize(ISession session, string table)
        {
            var config= new MappingConfiguration();
            config.Define(new Map<KeyValueDto>()
                .TableName(table)
                .PartitionKey(s => s.Id));

            session.Execute($"CREATE TABLE {table}(id text, data text, PRIMARY KEY (id));");
            
            return new CassandraKeyValueStore<TKey, TData>(session, config);
        }

        private readonly MappingConfiguration _mappingConfiguration;
        private readonly ISession _session;

        private CassandraKeyValueStore(ISession session, MappingConfiguration mappingConfiguration)
        {
            _session = session;
            _mappingConfiguration = mappingConfiguration;
        }

        public async Task<TData> GetAsync(TKey id)
        {
            var mapper = new Mapper(_session, _mappingConfiguration);
            var dto = await mapper.FirstOrDefaultAsync<KeyValueDto>("where id = ?", id);
            return dto?.ToData();
        }


        public async Task<IEnumerable<TData>> ListAsync()
        {
            var mapper = new Mapper(_session, _mappingConfiguration);
            var dto = await mapper.FetchAsync<KeyValueDto>();
            return dto?.Select(d => d.ToData());
        }

        public async Task SaveAsync(TKey id, TData data)
        {
            var mapper = new Mapper(_session, _mappingConfiguration);
            await mapper.InsertAsync(new KeyValueDto(){Id = id, Data = Newtonsoft.Json.JsonConvert.SerializeObject(data)});
        }

        public async Task RemoveAsync(TKey key)
        {
            var mapper = new Mapper(_session, _mappingConfiguration);
            await mapper.DeleteAsync<KeyValueDto>("where id = ?", key);
        }

        class KeyValueDto
        {

            public TKey Id { get; set; }
            public string Data { get; set; }

            public TData ToData()
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TData>(Data);
            }
        }
    }
}