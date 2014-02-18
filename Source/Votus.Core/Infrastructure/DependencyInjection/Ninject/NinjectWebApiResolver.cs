using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject;

namespace Votus.Core.Infrastructure.DependencyInjection.Ninject
{
    public class NinjectWebApiResolver : IDependencyResolver
    {
        readonly IKernel kernel;

        public NinjectWebApiResolver(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return kernel.GetAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }
    }
}