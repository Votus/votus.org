using Newtonsoft.Json;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Votus.Core.Domain.Goals;
using Votus.Core.Domain.Ideas;
using Votus.Core.Domain.Tasks;
using Votus.Core.Infrastructure.DependencyInjection.Ninject;
using Votus.Core.Infrastructure.EventSourcing;
using Votus.Core.Infrastructure.Logging;
using Votus.Core.Infrastructure.Queuing;
using Votus.Core.Infrastructure.Web.WebApi;
using Votus.Web.Areas.Api;

namespace Votus.Web
{
    public class Global : NinjectHttpApplication
    {
        #region Properties

        private ILog Log { get; set; }

        #endregion

        #region Overrides

        protected
        override 
        IKernel 
        CreateKernel()
        {
            // Create and load the Ninject kernel.
            var kernel = new StandardKernel(
                new ApiDependencyInjectionModule()
            );

            // Configure the DI for the WebAPI infrastructure.
            GlobalConfiguration
                .Configuration
                .DependencyResolver = new NinjectWebApiResolver(kernel);

            // Return the kernel for the GUI controllers.
            return kernel;
        }

        protected
        override
        void
        OnApplicationStarted()
        {
            Log = Get<ILog>();
            Log.Info("Votus Website Application starting...");

            ConfigureAspNet();
            SetupGlobalErrorLogging();

            RegisterCommandHandlers();
            BeginProcessingEvents();

            AreaRegistration.RegisterAllAreas();

            Log.Info("Votus Website Application started!");
        }

        protected
        override
        void
        OnApplicationStopped()
        {
            Log.Info("Votus Website Application stopping...");
            base.OnApplicationStopped();
            Log.Info("Votus Website Application stopped!");
        }

        #endregion

        #region Methods

        private
        static
        void
        ConfigureAspNet()
        {
            // Register all the MVC and WebApi areas.
            AreaRegistration.RegisterAllAreas();

            // Map all WebApi attribute routes.
            GlobalConfiguration.Configure(configuration => configuration.MapHttpAttributeRoutes());

            // Remove XML as the default content type to output on the API, should use JSON instead.
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            // Format all JSON output so that it is easier to read.
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
        }

        private 
        void
        SetupGlobalErrorLogging()
        {
            AppDomain.CurrentDomain.UnhandledException  += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException       += TaskScheduler_UnobservedTaskException;

            GlobalConfiguration.Configuration.Filters.Add(Get<UnhandledExceptionLoggerFilter>());
        }

        public
        void
        TaskScheduler_UnobservedTaskException(
            object                              sender, 
            UnobservedTaskExceptionEventArgs    e)
        {
            foreach (var exception in e.Exception.InnerExceptions)
            {
                Log.Error(exception);
            }
        }

        void
        CurrentDomain_UnhandledException(
            object                      sender, 
            UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject as Exception);
        }

        public 
        void 
        Application_Error(
            object      sender, 
            EventArgs   e)
        {
            var lastError = Server.GetLastError();

            if (Log == null)
            {
                Trace.TraceError(
                    "An error occurred while Log reference was null: sender: {0}, last error: {1}", 
                    sender, 
                    lastError
                );

                return;
            }

            Log.Error(lastError);
        }

        private 
        static
        void
        RegisterCommandHandlers()
        {
            // TODO: Implement some way to automatically discover/register command handlers.
            
            var eventStore   = Get<EventStore>();
            var ideasManager = Get<IdeasManager>();
            var goalsManager = Get<GoalsManager>();
            var tasksManager = Get<TasksManager>();
            var queueManager = Get<QueueManager>();
            
            queueManager.RegisterAsyncHandler<CreateIdeaCommand>(ideasManager.HandleAsync);
            queueManager.RegisterAsyncHandler<CreateGoalCommand>(goalsManager.HandleAsync);
            queueManager.RegisterAsyncHandler<CreateTaskCommand>(tasksManager.HandleAsync);
            queueManager.RegisterAsyncHandler<VoteTaskCompletedCommand>(tasksManager.HandleAsync);
            queueManager.RegisterAsyncHandler<RepublishAllEventsCommand>(e => eventStore.RepublishAllEventsAsync());

            queueManager.BeginProcessingMessages();
        }

        private 
        static 
        void 
        BeginProcessingEvents()
        {
            var eventManagers = GetMany<IEventProcessor>();

            // Not calling these synchronously on purpose so that the 
            // website can continue starting while the processing ramps up.
            foreach (var eventManager in eventManagers)
                eventManager.ProcessEventsAsync(); 
        }

        private
        static
        T
        Get<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        private
        static
        IEnumerable<T>
        GetMany<T>()
        {
            return DependencyResolver.Current.GetServices<T>();
        }

        #endregion
    }
}
