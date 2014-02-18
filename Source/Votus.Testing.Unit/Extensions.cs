using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Configuration;

namespace Votus.Testing.Unit
{
    static class Extensions
    {
        public
        static 
        IAfterCallSpecifiedWithOutAndRefParametersConfiguration
        ReturnsCompletedTask(
            this 
            IReturnValueArgumentValidationConfiguration<Task>  configuration)
        {
            return configuration.Returns(Task.Run(() => { }));
        }

        public
        static 
        IAfterCallSpecifiedWithOutAndRefParametersConfiguration
        ReturnsCompletedTask<T>(
            this 
            IReturnValueConfiguration<Task<T>>  configuration,
            T                                   returnValue)
        {
            return configuration.Returns(Task.Run(() => returnValue));
        }
    }
}
