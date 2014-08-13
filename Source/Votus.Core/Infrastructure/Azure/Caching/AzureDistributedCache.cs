using System;
using System.Security;
using Microsoft.ApplicationServer.Caching;
using Votus.Core.Infrastructure.Caching;

namespace Votus.Core.Infrastructure.Azure.Caching
{
    public class AzureDistributedCache : ICache
    {
        private readonly DataCache        _cache;
        private readonly DataCacheFactory _cacheFactory;

        private const string Region = "GlobalRegion";

        public 
        AzureDistributedCache(
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
        void 
        Set(
            string key,
            object value)
        {
            _cache.Put(
                key,
                value,
                Region
            );
        }

        public 
        void 
        Set(
            string          key, 
            object          value, 
            DateTimeOffset  expires,
            string          dependsOnKey)
        {
            var span = expires - DateTime.Now;

            if (dependsOnKey == null)
                dependsOnKey = key;

            _cache.Put(
                key,
                value,
                span,
                new[] { new DataCacheTag(dependsOnKey) },
                Region
            );
        }

        public 
        void 
        Add(
            string          key, 
            object          value, 
            DateTimeOffset  expires, 
            string          dependsOnKey = null)
        {
            var span = expires - DateTime.Now;

            if (dependsOnKey == null)
                dependsOnKey = key;
            
            _cache.Add(
                key,
                value, 
                span, 
                new[] { new DataCacheTag(dependsOnKey) }, 
                Region
            );
        }

        public 
        bool 
        Contains(
            string key)
        {
            return (_cache.Get(key, Region) != null);
        }

        public 
        object 
        Get(
            string key)
        {
            return _cache.Get(key, Region);
        }

        public 
        T 
        Get<T>(
            string key) where T: class
        {
            return (_cache.Get(key, Region) as T);
        }

        public 
        void 
        Remove(
            string key)
        {
            _cache.Remove(key, Region);
        }

        public 
        void 
        RemoveItemsStartingWith(
            string key)
        {
            var objectsByTag = _cache.GetObjectsByTag(new DataCacheTag(key), Region);

            foreach (var pair in objectsByTag)
                _cache.Remove(pair.Key, Region);
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
    }
}
