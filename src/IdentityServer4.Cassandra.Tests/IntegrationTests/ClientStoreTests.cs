using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer4.Models;
using NUnit.Framework;

namespace IdentityServer4.Cassandra.Tests.IntegrationTests
{
    [TestFixture(Category = "Integeration")]
    public class ClientStoreTests
    {
        private ISession _session;
        private static readonly string Keyspace =  typeof(ClientStoreTests).Name.ToLower();

        [SetUp]
        public void SetupSession()
        {

            _session = Cluster.Builder().AddContactPoint("localhost").Build().Connect();
            _session.Execute(
                $"CREATE KEYSPACE IF NOT EXISTS {Keyspace} WITH REPLICATION = {{ 'class' : 'SimpleStrategy', 'replication_factor' : 1 }};");
            _session.ChangeKeyspace(Keyspace);
        }


        [TearDown]
        public void Cleanup()
        {
            _session.Execute($"DROP KEYSPACE {Keyspace};");
        }

        [Test]
        public async Task StoresThenRetrievesByKey()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var clientsStore = await  stores.InitializeClientStore(new Client(){ClientId = "abc"});
            var client = await clientsStore.FindClientByIdAsync("abc");
            Assert.IsNotNull(client);
            Assert.AreEqual("abc", client.ClientId);
        }

    }
}