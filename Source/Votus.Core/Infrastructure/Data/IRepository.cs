namespace Votus.Core.Infrastructure.Data
{
    public interface IRepository<out T>
    {
        bool Exists(string key);
        T Get(string key);
        void Set(string key, object value);
        void DeleteStartsWith(string key);
    }
}
