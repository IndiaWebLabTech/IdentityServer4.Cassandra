using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Cassandra.Tests
{
    [Trait("Category","Integration")]
    public class ClientStoreTests
    {
        [Fact]
        public async Task StoresThenRetrievesByKey()
        {
            var clientsStore = new  CassandraClientStore(new MockKeyValueStore<String,Client>("abc", new Client{ClientId="abc"}));
            
            var client = await clientsStore.FindClientByIdAsync("abc");
            Assert.NotNull(client);
            Assert.Equal("abc", client.ClientId);
        }

    }
}