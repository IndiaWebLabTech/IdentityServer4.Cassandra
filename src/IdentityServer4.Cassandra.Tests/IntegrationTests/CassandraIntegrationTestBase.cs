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
    public class CassandraIntegrationTestBase : IDisposable
    {
        protected readonly ISession _session;
        private readonly string _keyspace;

        public CassandraIntegrationTestBase()
        {
            _keyspace =  this.GetType().Name.ToLower();
            _session = Cluster.Builder().AddContactPoint("localhost").Build().Connect();
            _session.Execute(
                $"CREATE KEYSPACE IF NOT EXISTS {_keyspace} WITH REPLICATION = {{ 'class' : 'SimpleStrategy', 'replication_factor' : 1 }};");
            _session.ChangeKeyspace(_keyspace);
        }


        public void Dispose()
        {
            try{
            _session.Execute($"DROP KEYSPACE {_keyspace};");
            }
            catch{}
            finally{
            _session.Dispose();
            }
        }
    }
}