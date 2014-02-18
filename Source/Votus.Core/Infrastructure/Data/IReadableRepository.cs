namespace Votus.Core.Infrastructure.Data
{
    public interface IReadableRepository
    {
        string Get(string settingName);
    }
}
