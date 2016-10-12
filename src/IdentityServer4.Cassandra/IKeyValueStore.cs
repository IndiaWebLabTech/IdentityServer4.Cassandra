using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Cassandra
{
    public interface IKeyValueStore<TKey,TValue> where TValue : class
    {
        Task<TValue> GetAsync(TKey id);
        Task<IEnumerable<TValue>> ListAsync();
        Task SaveAsync(TKey id, TValue data);
        Task RemoveAsync(TKey key);
    }
}