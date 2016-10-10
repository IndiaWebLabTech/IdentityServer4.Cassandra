using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using NUnit.Framework;

namespace IdentityServer4.Cassandra.Tests.IntegrationTests
{
    [TestFixture(Category = "Integeration")]
    public class ScopeStoreIntegrationTests
    {
        private ISession _session;
        private static readonly string Keyspace =  typeof(ScopeStoreIntegrationTests).Name.ToLower();

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
        public async Task StoresThenRetrievesScopesByName()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var scopesStore = await  stores.InitializeScopeStoreAsync(new Scope(){Name = "123"});
            var scopes = await scopesStore.FindScopesAsync(new[] {"123"});
            Assert.IsNotEmpty(scopes);
            Assert.AreEqual("123", scopes.Single().Name);
        }

    }
}