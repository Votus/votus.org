using System;

namespace Votus.Core.Infrastructure.Data
{
    public class EnvironmentVariableRepository : IReadWriteRepository
    {
        public 
        string 
        Get(
            string settingName)
        {
            return 
                Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.Process) ??
                Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.User);
        }

        public 
        void 
        Set(
            string settingName, 
            string value)
        {
            Environment.SetEnvironmentVariable(
                settingName, 
                value,
                EnvironmentVariableTarget.User
            );
        }
    }
}
