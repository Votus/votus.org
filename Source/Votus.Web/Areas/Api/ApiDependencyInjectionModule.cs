﻿using Ninject;
using Ninject.Modules;
using Strathweb.CacheOutput.WebApi2.Azure;
using System;
using Votus.Core;
using Votus.Core.Domain.Goals;
using Votus.Core.Domain.Ideas;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.Azure.Caching;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Web.Areas.Api.ViewManagers;
using WebApi.OutputCache.Core.Cache;

namespace Votus.Web.Areas.Api
{
    public class ApiDependencyInjectionModule : NinjectModule
    {
        public override void Load()
        {
            // Configure all the core dependencies...
            Kernel.Load(new[] { new CoreInjectionModule() });

            // Bind the view caches...
            Bind<IPartitionedRepository>()
                .ToMethod(ctx =>
                    CoreInjectionModule.CreatePartitionedRepo(ctx, "WebsiteIdeasReverseChronologicalCache") // TODO: Rename to match view manager.
                )
                .InSingletonScope();

            // Configure the web api output caching provider
            Bind<IApiOutputCache>()
                .ToMethod(ctx =>
                          {
                              var config = ctx.Kernel.Get<ApplicationSettings>();

                              return new AzureCachingProvider(
                                  config.AzureCacheServiceName,
                                  config.AzureCacheServicePrimaryAccessKey
                              );
                          })
                .InSingletonScope();

            Bind<IRepository<VoteTaskCompletedCommand>>()
                .ToMethod(ctx => {
                    var config = ctx.Kernel.Get<ApplicationSettings>();
                    
                    return new DataCacheRepository<VoteTaskCompletedCommand>(
                        azureCacheServiceName: config.AzureCacheServiceName,
                        azureCacheServiceKey:  config.AzureCacheServicePrimaryAccessKey
                    );
                })
                .InSingletonScope();

            // Bind all the events to their handlers...
            BindEvent<IdeaCreatedEvent      >(Kernel.Get<IdeasByTimeDescendingViewManager>().HandleAsync);
            BindEvent<IdeaCreatedEvent      >(Kernel.Get<IdeaByIdViewManager             >().HandleAsync);
            BindEvent<GoalCreatedEvent      >(Kernel.Get<IdeasManager                    >().HandleAsync); // TODO: Bind in Core
            BindEvent<TaskCreatedEvent      >(Kernel.Get<IdeasManager                    >().HandleAsync); // TODO: Bind in Core
            BindEvent<TaskCreatedEvent      >(Kernel.Get<TaskByIdViewManager             >().HandleAsync);
            BindEvent<TaskVotedCompleteEvent>(Kernel.Get<TaskByIdViewManager             >().HandleAsync);
            BindEvent<TaskVotedCompleteEvent>(Kernel.Get<TasksByIdeaViewManager          >().HandleAsync);
            BindEvent<TaskAddedToIdeaEvent  >(Kernel.Get<TasksByIdeaViewManager          >().HandleAsync);
            BindEvent<GoalAddedToIdeaEvent  >(Kernel.Get<GoalsByIdeaViewManager          >().HandleAsync);
        }

        public
        void BindEvent<TEvent>(
            Func<TEvent, System.Threading.Tasks.Task> handler)
        {
            Bind<IEventProcessor>()
                .ToMethod(ctx => 
                    new ServiceBusSubscriptionProcessor<TEvent>(
                        serviceBusConnectionString: ctx.Kernel.Get<ApplicationSettings>().AzureServiceBusConnectionString,
                        topicPath:                  CoreInjectionModule.AggregateRootEventsTopicPath,
                        asyncEventHandler:          handler
                    )
                )
                .InSingletonScope();
        }
    }
}