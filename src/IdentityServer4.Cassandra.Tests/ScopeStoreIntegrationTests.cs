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
            var scopesStore = new CassandraScopeStore(new MockKeyValueStore<string,Scope>("123", new Scope{Name="123"}));
            var scopes = await scopesStore.FindScopesAsync(new[] {"123"});
            Assert.NotEmpty(scopes);
            Assert.Equal("123", scopes.Single().Name);
        }

    }
}