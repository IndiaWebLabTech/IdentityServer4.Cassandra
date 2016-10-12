using System;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Cassandra.Tests.IntegrationTests
{
    [Trait("Category","Integeration")]
    public class ScopeStoreIntegrationTests : CassandraIntegrationTestBase
    {

        [Fact]
        public async Task StoresThenRetrievesScopesByName()
        {
            var scopesStore = await  CassandraIdentityServerStores.InitializeScopeStoreAsync(_session, new Scope(){Name = "123"});
            var scopes = await scopesStore.FindScopesAsync(new[] {"123"});
            Assert.NotEmpty(scopes);
            Assert.Equal("123", scopes.Single().Name);
        }

    }
}