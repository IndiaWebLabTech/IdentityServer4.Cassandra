using System.Collections.Generic;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer4.Cassandra
{
    public class CassandraClientStore : IClientStore
    {
        private const string TABLE_NAME = "IdentityServer4_Clients";
        private const string SCHEMA_INITIALIZATION = @"
CREATE TABLE IF NOT EXISTS {0}(clientid text, data text, PRIMARY KEY (clientid));";

        static CassandraClientStore()
        {
            MappingConfiguration.Global.Define(new Map<ClientDto>()
                .TableName(TABLE_NAME)
                .PartitionKey(s => s.ClientId));
        }

        private readonly ISession _session;

        public CassandraClientStore(ISession session)
        {
            _session = session;
        }

        internal async Task InitializeAsync(params Client[] clients)
        {
            var createSchemaCql = string.Format(SCHEMA_INITIALIZATION, TABLE_NAME);
            await _session.ExecuteAsync(_session.Prepare(createSchemaCql).Bind());
            var mapper = new Mapper(_session);
            var insertTasks = new List<Task>();
            foreach(var client in clients)
            {
                insertTasks.Add(mapper.InsertAsync(ClientDto.FromClient(client)));
            }
            await Task.WhenAll(insertTasks);
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var mapper = new Mapper(_session);
            var dto = await mapper.FirstOrDefaultAsync<ClientDto>("where clientid = ?", clientId);
            return dto.ToClient();
        }

        class ClientDto
        {

            public string ClientId { get; set; }
            public string Data { get; set; }

            public Client ToClient()
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Client>(Data);
            }

            public static ClientDto FromClient(Client client)
            {
                return new ClientDto()
                {
                    ClientId = client.ClientId,
                    Data = Newtonsoft.Json.JsonConvert.SerializeObject(client)
                };
            }
        }
    }
}