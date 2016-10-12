using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Cassandra.Tests
{
    [Trait("Category","Integration")]
    public class PersistedGrantStoreTests
    {

        [Fact]
        public async Task StoresThenRetrievesByKey()
        {
            var grantsStore = new CassandraPersistedGrantStore(new MockKeyValueStore<string, PersistedGrant>());
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "123"});
            var storedGrant = await grantsStore.GetAsync("123");
            Assert.NotNull(storedGrant);
            Assert.Equal("123", storedGrant.Key);
        }

        [Fact]
        public async Task StoresThenDeletesByKey()
        {
            var grantsStore = new CassandraPersistedGrantStore(new MockKeyValueStore<string, PersistedGrant>());
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "999"});
            var storedGrant = await grantsStore.GetAsync("999");
            Assert.NotNull(storedGrant);
            Assert.Equal("999", storedGrant.Key);
        }

        [Fact]
        public async Task StoresThenFetchesBySubject()
        {
            var grantsStore = new CassandraPersistedGrantStore(new MockKeyValueStore<string, PersistedGrant>());
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "321", SubjectId = "Some App"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "456", SubjectId = "Some App"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "789", SubjectId = "Some Other App"});
            var storedGrants = await grantsStore.GetAllAsync("Some App");
            Assert.Equal(2, storedGrants.Count());
        }

        [Fact]
        public async Task StoresThenRemovesBySubjectAndClient()
        {
            var grantsStore = new CassandraPersistedGrantStore(new MockKeyValueStore<string, PersistedGrant>());
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "666", SubjectId = "Some App", ClientId = "mt"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "111", SubjectId = "Some App", ClientId = "jp"});
            await grantsStore.RemoveAllAsync("Some App", "mt");
            var storedGrants = await grantsStore.GetAllAsync("Some App");
            storedGrants = storedGrants.ToArray();
            Assert.Equal(1, storedGrants.Count());
            Assert.Equal("jp", storedGrants.Single().ClientId);
        }

        [Fact]
        public async Task StoresThenRemovesBySubjectAndClientAndType()
        {
            var grantsStore = new CassandraPersistedGrantStore(new MockKeyValueStore<string, PersistedGrant>());
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "666", SubjectId = "Some App", ClientId = "mt", Type = "User"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "111", SubjectId = "Some App", ClientId = "mt", Type = "Admin"});
            await grantsStore.StoreAsync(new PersistedGrant() {Key = "456", SubjectId = "Some App", ClientId = "jp", Type = "User"});
            await grantsStore.RemoveAllAsync("Some App", "mt", "User");
            var storedGrants = await grantsStore.GetAllAsync("Some App");
            storedGrants = storedGrants.ToArray();
            Assert.Equal(2, storedGrants.Count());
        }
    }
}