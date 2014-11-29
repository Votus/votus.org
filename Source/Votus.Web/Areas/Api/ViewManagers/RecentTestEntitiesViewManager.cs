using Ninject;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votus.Core.Domain.TestEntities;
using Votus.Core.Infrastructure.Data;
using Votus.Web.Areas.Api.Controllers;
using WebApi.OutputCache.Core.Cache;
using WebApi.OutputCache.V2;

namespace Votus.Web.Areas.Api.ViewManagers
{
    public class RecentTestEntitiesViewManager
    {
        [Inject] public IKeyValueRepository         ViewCache       { get; set; }
        [Inject] public IApiOutputCache             OutputCache     { get; set; } // TODO: Consolidate caching logic so two separate properties aren't needed.
        [Inject] public CacheOutputConfiguration    CacheConfig     { get; set; }

        public 
        async Task
        HandleAsync(
            TestEntityCreatedEvent testEntityCreatedEvent)
        {
            var currentList = await ViewCache.GetAsync<List<TestEntityViewModel>>("recent-test-entities") ?? new List<TestEntityViewModel>();

            var newTestEntity = new TestEntityViewModel {
                Id           = testEntityCreatedEvent.EventSourceId, 
                TestProperty = testEntityCreatedEvent.TestProperty
            };

            currentList.Add(newTestEntity);

            // TODO: Need to do some concurrency control somewhere for updating this data.
            await ViewCache.SetAsync("recent-test-entities", currentList);
            
            // TODO: Abstract away the implementation details of this cache key stuff...
            var cacheKey = CacheConfig.MakeBaseCachekey((InfrastructureTestingController c) => c.GetRecentTestEntities());
            OutputCache.RemoveStartsWith(cacheKey);
        }
    }
}