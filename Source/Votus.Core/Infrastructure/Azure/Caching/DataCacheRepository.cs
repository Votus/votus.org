using Microsoft.ApplicationServer.Caching;
using System;
using System.Security;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Infrastructure.Azure.Caching
{
    public class DataCacheRepository<T> : IRepository<T>
    {
        private readonly DataCache          _cache;
        private readonly DataCacheFactory   _cacheFactory;

        private const string Region = "GlobalRegion";

        public 
        DataCacheRepository(
            string azureCacheServiceName,
            string azureCacheServiceKey,
            string cacheName = "default")
        {
            var cacheAddress = string.Format("{0}.cache.windows.net", azureCacheServiceName);

            var configuration = new DataCacheFactoryConfiguration {
                SecurityProperties = new DataCacheSecurity(
                    authorizationToken: ToSecureString(azureCacheServiceKey),
                    sslEnabled:         true),
                AutoDiscoverProperty = new DataCacheAutoDiscoverProperty(
                    enable:             true, 
                    identifier:         cacheAddress),
                LocalCacheProperties = new DataCacheLocalCacheProperties(
                    objectCount:        10000,
                    defaultTimeout:     TimeSpan.FromMinutes(1.0),
                    invalidationPolicy: DataCacheLocalCacheInvalidationPolicy.NotificationBased),
                MaxConnectionsToServer = 3 // Not sure what this number should be yet...
            };

            _cacheFactory = new DataCacheFactory(configuration);
            _cache        = _cacheFactory.GetCache(cacheName);

            _cache.CreateRegion(Region);
        } 

        public 
        static 
        SecureString 
        ToSecureString(
            string unsecureString)
        {
            var str = new SecureString();

            foreach (var ch in unsecureString)
                str.AppendChar(ch);

            str.MakeReadOnly();

            return str;
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
