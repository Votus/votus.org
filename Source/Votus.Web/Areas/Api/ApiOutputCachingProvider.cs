using System;
using Ninject;
using Votus.Core.Infrastructure.Caching;
using WebApi.OutputCache.Core.Cache;

namespace Votus.Web.Areas.Api
{
    /// <summary>
    /// Allows the ICache to be used for API output caching.
    /// </summary>
    public class ApiOutputCachingProvider : IApiOutputCache
    {
        [Inject] public ICache Cache { get; set; }

        #region IApiOutputCache Members

        public 
        void 
        RemoveStartsWith(
            string key)
        {
            Cache.RemoveItemsStartingWith(key);
        }

        public 
        T 
        Get<T>(
            string key) where T : class
        {
            return Cache.Get<T>(key);
        }

        public 
        object 
        Get(
            string key)
        {
            return Cache.Get(key);
        }

        public 
        void 
        Remove(
            string key)
        {
            Cache.Remove(key);
        }

        public 
        bool 
        Contains(
            string key)
        {
            return Cache.Contains(key);
        }

        public 
        void 
        Add(
            string          key, 
            object          o,
            DateTimeOffset  expiration, 
            string          dependsOnKey = null)
        {
            Cache.Set(key, o, expiration, dependsOnKey);
        }

        #endregion
    }
}