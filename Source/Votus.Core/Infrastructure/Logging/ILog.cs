
namespace Votus.Core.Infrastructure.Logging
{
    public interface ILog
    {
        void Error  (object message);
        void Info   (object message);
        void Verbose(object message, params object[] args);
        void Warning(object message, params object[] args);
    }
}