using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Cassandra.Tests
{
    [Trait("Category","Integeration")]
    public class ScopeStoreIntegrationTests
    {

        [Fact]
        public async Task StoresThenRetrievesScopesByName()
        {
            var mockIdStore = new MockKeyValueStore<string,IdentityResource>("123", new IdentityResource("123", new[]{"openid"}));
            var mockApiStore = new MockKeyValueStore<string,ApiResource>("api123", new ApiResource("api123"));
            var resourceStore = new CassandraResourceStore(mockIdStore, mockApiStore);
            var apiResource = await resourceStore.FindApiResourceAsync("api123");
            var idResource = await resourceStore.FindIdentityResourcesByScopeAsync(new[]{"123"});
            Assert.NotNull(apiResource);
            Assert.NotEmpty(idResource);
            Assert.Equal("123", idResource.First().Name);
            Assert.Equal("api123", apiResource.Name);
        }

    }
}