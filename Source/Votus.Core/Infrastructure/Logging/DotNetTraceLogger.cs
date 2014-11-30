using System.Diagnostics;

namespace Votus.Core.Infrastructure.Logging
{
    public class DotNetTraceLogger : ILog
    {
        public 
        void 
        Error(
            object message,
            params object[] args)
        {
            Trace.TraceError(FormatMessage(message, args));
        }

        public 
        void 
        Info(
            object          message, 
            params object[] args)
        {
            Trace.TraceInformation(FormatMessage(message, args));
        }

        public 
        void 
        Verbose(
            object message, 
            params object[] args)
        {
            Trace.WriteLine(FormatMessage(message, args));
        }

        public 
        void 
        Warning(
            object message, 
            params object[] args)
        {
            Trace.TraceWarning(FormatMessage(message, args));
        }

        static
        string
        FormatMessage(
            object      message, 
            object[]    args)
        {
            var formattedMessage = message.ToString();

            if (args.Length > 0)
                formattedMessage = string.Format(formattedMessage, args);

            return formattedMessage;
        }
    }
}
