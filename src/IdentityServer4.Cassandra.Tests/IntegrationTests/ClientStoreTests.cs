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
    public class ClientStoreTests : CassandraIntegrationTestBase
    {
        [Fact]
        public async Task StoresThenRetrievesByKey()
        {
            var clientsStore = await  CassandraIdentityServerStores.InitializeClientStore(_session, new Client(){ClientId = "abc"});
            var client = await clientsStore.FindClientByIdAsync("abc");

            Assert.NotNull(client);
            Assert.Equal("abc", client.ClientId);
        }

    }
}