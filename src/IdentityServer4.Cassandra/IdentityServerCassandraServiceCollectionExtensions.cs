

using Cassandra;
using IdentityServer4.Cassandra;
using IdentityServer4.Models;
using IdentityServer4.Stores;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerCassandraServiceCollectionExtensions
    {
        public static IIdentityServerBuilder AddCassandraScopes(this IIdentityServerBuilder builder, ISession session,
            params Scope[] scopes)
        {
            var store = CassandraIdentityServerStores.InitializeScopeStoreAsync(session, scopes)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            builder.Services.AddSingleton<IScopeStore>(store);
            return builder;
        }

        public static IIdentityServerBuilder AddCassandraClients(this IIdentityServerBuilder builder, ISession session,
            params Client[] clients)
        {
            var store = CassandraIdentityServerStores.InitializeClientStore(session, clients)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            builder.Services.AddSingleton<IClientStore>(store);
            return builder;
        }

        public static IIdentityServerBuilder AddCassandraPersistedGrantStore(this IIdentityServerBuilder builder, ISession session)
        {
            var store = CassandraIdentityServerStores.InitializeGrantsStoreAsync(session)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            builder.Services.AddSingleton<IPersistedGrantStore>(store);
            return builder;
        }

        public static IIdentityServerBuilder AddCassandraScopes(this IIdentityServerBuilder builder, params Scope[] scopes)
        {
            builder.Services.AddSingleton<IScopeStore>(c =>
            {
                var session = c.GetRequiredService<ISession>();
                return  CassandraIdentityServerStores.InitializeScopeStoreAsync(session, scopes)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            });

            return builder;
        }

        public static IIdentityServerBuilder AddCassandraClients(this IIdentityServerBuilder builder, params Client[] clients)
        {
            builder.Services.AddSingleton<IClientStore>(c =>
            {
                var session = c.GetRequiredService<ISession>();
                return CassandraIdentityServerStores.InitializeClientStore(session, clients)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            });

            return builder;
        }

        public static IIdentityServerBuilder AddCassandraPersistedGrantStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IPersistedGrantStore>(c =>
            {
                var session = c.GetRequiredService<ISession>();
                return CassandraIdentityServerStores.InitializeGrantsStoreAsync(session)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            });
            return builder;
        }
    }
}