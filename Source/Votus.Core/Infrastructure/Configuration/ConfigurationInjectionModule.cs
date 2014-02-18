using Ninject.Modules;
using System;
using System.Collections.Generic;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Infrastructure.Configuration
{
    public class ConfigurationInjectionModule : NinjectModule
    {
        private readonly Type[] _types;

        public ConfigurationInjectionModule(params Type[] providerTypes)
        {
            _types = providerTypes;
        }

        public override void Load()
        {
            foreach (var type in _types)
                Bind<IReadableRepository>().To(type);
            
            Bind<IEnumerable<IReadableRepository>>()
                .ToSelf()
                .InSingletonScope();

            Bind<ConfigManager>()
                .ToSelf()
                .InSingletonScope();
        }
    }
}
