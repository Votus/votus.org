using System;

namespace Votus.Core.Infrastructure.Configuration
{
    public class SaveSettingException : Exception
    {
        public SaveSettingException(string message) :base(message) { }
    }
}
