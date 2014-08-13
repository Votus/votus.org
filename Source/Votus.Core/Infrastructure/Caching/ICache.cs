using System;
using Votus.Core.Domain.Tasks;

namespace Votus.Core.Infrastructure.Caching
{
    public interface ICache
    {
        bool Contains(string key);

        object  Get(   string key);
        T       Get<T>(string key) where T: class;

        void Remove(                 string key);
        void RemoveItemsStartingWith(string key);

        void Set(string key, object value);
        void Set(string key, object value, DateTimeOffset expiration, string dependsOnKey);

        void Add(string key, object value, DateTimeOffset expires, string dependsOnKey);
    }
}