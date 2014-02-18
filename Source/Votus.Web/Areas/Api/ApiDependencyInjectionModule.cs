using Ninject.Modules;
using Votus.Core;
using Votus.Core.Infrastructure.Data;

namespace Votus.Web.Areas.Api
{
    public class ApiDependencyInjectionModule : NinjectModule
    {
        public override void Load()
        {
            // Configure all the core dependencies...
            Kernel.Load(new[] { new CoreInjectionModule() });

            // Configure the api-specific dependencies...
            Bind<IPartitionedRepository>()
                .ToMethod(ctx => 
                    CoreInjectionModule.CreatePartitionedRepo(ctx, "WebsiteIdeasReverseChronologicalCache")
                )
                .InSingletonScope();
        }
    }
}