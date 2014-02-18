using System.Net;
using Ninject;
using System;

namespace Votus.Testing.Integration
{
    static class SharedResources
    {
        private static readonly IKernel _kernel;
        private static readonly string _testId = Guid.NewGuid().ToString("N");

        public static string TestId
        {
            get { return _testId; }
        }

        static SharedResources()
        {
            ServicePointManager.DefaultConnectionLimit = 10;
            _kernel = ConfigureDependencyInjection();
        }

        private 
        static 
        IKernel 
        ConfigureDependencyInjection()
        {
            return new StandardKernel(
                new IntegrationTestingInjectionModule()
            );
        }

        public 
        static 
        T 
        Get<T>()
        {
            return _kernel.Get<T>();
        }
    }
}