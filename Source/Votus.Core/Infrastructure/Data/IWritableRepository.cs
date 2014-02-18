namespace Votus.Core.Infrastructure.Data
{
    public interface IWritableRepository
    {
        void Set(string id, string value);
    }
}
