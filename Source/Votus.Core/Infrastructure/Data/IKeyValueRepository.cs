using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.Data
{
    public interface IKeyValueRepository
    {
        Task         SetAsync<TValue>(object key, TValue value);
        Task<TValue> GetAsync<TValue>(object key);
    }
}