using Microsoft.ApplicationServer.Caching;
using System;
using System.Security;
using WebApi.OutputCache.Core.Cache;

namespace Votus.Web.Infrastructure.Caching.Azure
{
    // TODO: Refactor this class with DataCacheRepository to eliminate duplication.

    public class AzureCachingProvider : IApiOutputCache
    {
        private readonly DataCache        _cache;
        private readonly DataCacheFactory _cacheFactory;

        private const string Region = "GlobalRegion";

        public 
        AzureCachingProvider(
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
        Add(
            string          key, 
            object          o, 
            DateTimeOffset  expiration, 
            string          dependsOnKey = null)
        {
            var span = expiration - DateTime.Now;

            if (dependsOnKey == null)
                dependsOnKey = key;
            
            _cache.Put(
                key,
                o, 
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
        RemoveStartsWith(
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