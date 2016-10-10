using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using IdentityServer4.Models;
using NUnit.Framework;

namespace IdentityServer4.Cassandra.Tests.IntegrationTests
{
    [TestFixture(Category = "Integeration")]
    public class PersistedGrantStoreTests
    {
        private ISession _session;
        private static readonly string Keyspace =  typeof(PersistedGrantStoreTests).Name.ToLower();

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
            var grantsStore = await  stores.InitializeGrantsStoreAsync();
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "123"});
            var storedGrant = await grantsStore.GetAsync("123");
            Assert.IsNotNull(storedGrant);
            Assert.AreEqual("123", storedGrant.Key);
        }

        [Test]
        public async Task StoresThenDeletesByKey()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var grantsStore = await  stores.InitializeGrantsStoreAsync();
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "999"});
            var storedGrant = await grantsStore.GetAsync("999");
            Assert.IsNotNull(storedGrant);
            Assert.AreEqual("999", storedGrant.Key);
        }

        [Test]
        public async Task StoresThenFetchesBySubject()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var grantsStore = await  stores.InitializeGrantsStoreAsync();
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "321", SubjectId = "Some App"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "456", SubjectId = "Some App"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "789", SubjectId = "Some Other App"});
            var storedGrants = await grantsStore.GetAllAsync("Some App");
            Assert.AreEqual(2, storedGrants.Count());
        }

        [Test]
        public async Task StoresThenRemovesBySubjectAndClient()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var grantsStore = await  stores.InitializeGrantsStoreAsync();
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "666", SubjectId = "Some App", ClientId = "mt"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "111", SubjectId = "Some App", ClientId = "jp"});
            await grantsStore.RemoveAllAsync("Some App", "mt");
            var storedGrants = await grantsStore.GetAllAsync("Some App");
            storedGrants = storedGrants.ToArray();
            Assert.AreEqual(1, storedGrants.Count());
            Assert.AreEqual("jp", storedGrants.Single().ClientId);
        }

        [Test]
        public async Task StoresThenRemovesBySubjectAndClientAndType()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var grantsStore = await  stores.InitializeGrantsStoreAsync();
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "666", SubjectId = "Some App", ClientId = "mt", Type = "User"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "111", SubjectId = "Some App", ClientId = "mt", Type = "Admin"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "456", SubjectId = "Some App", ClientId = "jp", Type = "User"});
            await grantsStore.RemoveAllAsync("Some App", "mt", "User");
            var storedGrants = await grantsStore.GetAllAsync("Some App");
            storedGrants = storedGrants.ToArray();
            Assert.AreEqual(2, storedGrants.Count());
        }
    }
}