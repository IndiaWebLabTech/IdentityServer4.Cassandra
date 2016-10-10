using System;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.Cassandra.Tests.IntegrationTests
{
    [Trait("Category","Integeration")]
    public class ScopeStoreIntegrationTests : IDisposable
    {
        private readonly ISession _session;
        private static readonly string Keyspace =  typeof(ScopeStoreIntegrationTests).Name.ToLower();

        public ScopeStoreIntegrationTests()
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
        public async Task StoresThenRetrievesScopesByName()
        {
            var stores = new CassandraIdentityServerStores(_session);
            var scopesStore = await  stores.InitializeScopeStoreAsync(new Scope(){Name = "123"});
            var scopes = await scopesStore.FindScopesAsync(new[] {"123"});
            Assert.NotEmpty(scopes);
            Assert.Equal("123", scopes.Single().Name);
        }

    }
}