using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Cassandra.Tests
{
    class MockKeyValueStore<TKey, TValue> : IKeyValueStore<TKey, TValue> where TValue : class
    {
        private readonly IDictionary<TKey, TValue> _store = new Dictionary<TKey, TValue>();

        public MockKeyValueStore()
        {
            _store = new Dictionary<TKey, TValue>();
        }

        public MockKeyValueStore(TKey key, TValue @value)
        {
            _store = new Dictionary<TKey,TValue>{{key,@value}};
        }

        public MockKeyValueStore(IDictionary<TKey,TValue> store)
        {
            _store = store;
        }


        public Task<TValue> GetAsync(TKey id)
        {
            return Task.FromResult(_store[id]);
        }

        public Task<IEnumerable<TValue>> ListAsync()
        {
            return Task.FromResult(_store.Values.AsEnumerable());
        }

        public Task RemoveAsync(TKey key)
        {
            _store.Remove(key);
            return Task.FromResult(0);
        }

        public Task SaveAsync(TKey id, TValue data)
        {
            _store[id] = data;
            return Task.FromResult(0);
        }

        public IDictionary<TKey, TValue> Store => _store;
    }
}