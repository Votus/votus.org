using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.Data
{
    public interface IKeyValueRepository
    {
        Task         SetAsync<TValue>(string key, TValue value);
        Task<TValue> GetAsync<TValue>(string key);
    }
}