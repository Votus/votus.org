
namespace Votus.Core.Infrastructure.Logging
{
    public interface ILog
    {
        void Error  (object message, params object[] args);
        void Info   (object message, params object[] args);
        void Verbose(object message, params object[] args);
        void Warning(object message, params object[] args);
    }
}