using System;

namespace Votus.Core.Infrastructure.Configuration
{
    public class SettingNotFoundException : Exception
    {
        public SettingNotFoundException(string settingName)
            : base(
                string.Format("A setting named '{0}' could not be found.", settingName))
        {
        }
    }
}
