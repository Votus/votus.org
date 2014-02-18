using System;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.EventSourcing;

namespace Votus.Core.Infrastructure.Data
{
    public interface IVersioningRepository<in TEntity>
    {
        Task<T> GetAsync<T>(Guid id) where T : AggregateRoot, new();
        Task    SaveAsync(TEntity entity, int expectedVersion = 0);
    }
}
