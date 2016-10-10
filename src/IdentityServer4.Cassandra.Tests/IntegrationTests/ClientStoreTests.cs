using System;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Cassandra.Tests.IntegrationTests
{
    [Trait("Category","Integration")]
    public class ClientStoreTests : IDisposable
    {
        private readonly ISession _session;
        private static readonly string Keyspace =  typeof(ClientStoreTests).Name.ToLower();

        public ClientStoreTests()
        {

            _session = Cluster.Builder().AddContactPoint("localhost").Build().Connect();
            _session.Execute(
                $"CREATE KEYSPACE IF NOT EXISTS {Keyspace} WITH REPLICATION = {{ 'class' : 'SimpleStrategy', 'replication_factor' : 1 }};");
            _session.ChangeKeyspace(Keyspace);
        }


        public void Dispose()
        {
            _session.Execute($"DROP KEYSPACE {Keyspace};");
        }

        [Fact]
        public async Task StoresThenRetrievesByKey()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var clientsStore = await  stores.InitializeClientStore(new Client(){ClientId = "abc"});
            var client = await clientsStore.FindClientByIdAsync("abc");

            Assert.NotNull(client);
            Assert.Equal("abc", client.ClientId);
        }

    }
}