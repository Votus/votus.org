using System;
using Microsoft.ApplicationServer.Caching;
using System.Security;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Infrastructure.Azure.Caching
{
    public class DataCacheRepository<T> : IRepository<T>
    {
        private readonly DataCache _cache;
        private const string Region = "GlobalRegion";

        public 
        DataCacheRepository(
            string azureCacheServiceName,
            string azureCacheServiceKey)
        {
            // Declare array for cache host.
            var secureACSKey = new SecureString();
            foreach (var a in azureCacheServiceKey)
                secureACSKey.AppendChar(a);
            
            secureACSKey.MakeReadOnly();

            var config = new DataCacheFactoryConfiguration {
                SecurityProperties      = new DataCacheSecurity(secureACSKey),
                AutoDiscoverProperty    = new DataCacheAutoDiscoverProperty(true, azureCacheServiceName + ".cache.windows.net")
            };

            var dataCacheFactory = new DataCacheFactory(config);

            _cache = dataCacheFactory.GetCache("default");
            _cache.CreateRegion(Region);
        } 

        public 
        bool 
        Exists(
            string key)
        {
            return Get(key) != null;
        }

        public T Get(string key)
        {
            return (T)_cache.Get(key, Region);
        }

        public
        void 
        Set(
            string key, 
            object value)
        {
            _cache.Put(key, value, new[] { new DataCacheTag(key) }, Region);
        }

        public 
        void 
        DeleteStartsWith(
            string key)
        {
            var objects = _cache.GetObjectsByTag(
                new DataCacheTag(key),
                Region
            );

            foreach (var obj in objects)
                _cache.Remove(obj.Key, Region);
        }
    }
}
