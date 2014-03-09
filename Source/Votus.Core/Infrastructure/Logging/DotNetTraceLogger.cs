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
            var formattedMessage = message.ToString();

            if (args.Length > 0)
                formattedMessage = string.Format(formattedMessage, args);

            Trace.TraceError(formattedMessage);
        }

        public 
        void 
        Info(
            object message)
        {
            Trace.TraceInformation(message.ToString());
        }

        public 
        void 
        Verbose(
            object message, 
            params object[] args)
        {
            var formattedMessage = message.ToString();

            if (args.Length > 0)
                formattedMessage = string.Format(formattedMessage, args);

            Trace.WriteLine(formattedMessage);
        }

        public 
        void 
        Warning(
            object message, 
            params object[] args)
        {
            var formattedMessage = message.ToString();

            if (args.Length > 0)
                formattedMessage = string.Format(formattedMessage, args);

            Trace.TraceWarning(formattedMessage);
        }
    }
}
