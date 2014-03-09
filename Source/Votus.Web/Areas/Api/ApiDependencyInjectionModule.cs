using Ninject;
using Ninject.Modules;
using System;
using Votus.Core;
using Votus.Core.Goals;
using Votus.Core.Ideas;
using Votus.Core.Infrastructure.Azure.ServiceBus;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Tasks;
using Votus.Web.Areas.Api.ViewManagers;

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
                    CoreInjectionModule.CreatePartitionedRepo(ctx, "WebsiteIdeasReverseChronologicalCache")
                )
                .InSingletonScope();

            // Bind all the events to their handlers...
            BindEvent<IdeaCreatedEvent      >(Kernel.Get<IdeasViewManager        >().HandleAsync);
            BindEvent<GoalCreatedEvent      >(Kernel.Get<IdeasManager            >().HandleAsync); // TODO: Bind in Core
            BindEvent<TaskCreatedEvent      >(Kernel.Get<IdeasManager            >().HandleAsync); // TODO: Bind in Core
            BindEvent<TaskCreatedEvent      >(Kernel.Get<TasksByTaskIdViewManager>().HandleAsync);
            BindEvent<TaskVotedCompleteEvent>(Kernel.Get<TasksByTaskIdViewManager>().HandleAsync);
            BindEvent<TaskVotedCompleteEvent>(Kernel.Get<IdeaTasksViewManager    >().HandleAsync);
            BindEvent<TaskAddedToIdeaEvent  >(Kernel.Get<IdeaTasksViewManager    >().HandleAsync);
            BindEvent<GoalAddedToIdeaEvent  >(Kernel.Get<IdeaGoalsViewManager    >().HandleAsync);
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