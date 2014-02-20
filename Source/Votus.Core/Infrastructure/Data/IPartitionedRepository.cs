using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.Data
{
    public interface IPartitionedRepository
    {
        Task<IEnumerable<TEntity>> GetAllAsync       <TEntity>();
        Task<PagedResult<TEntity>> GetAllPagedAsync  <TEntity>(                                    string nextPageToken = null, int maxPerPage = 1000);
        Task<PagedResult<TEntity>> GetWherePagedAsync<TEntity>(Func<TEntity, bool> wherePredicate, string nextPageToken = null, int maxPerPage = 1000);
        Task<IEnumerable<TEntity>> GetPartitionAsync <TEntity>(object partitionKey);

        Task InsertAsync     <TEntity>(object partitionKey,               object  rowKey, TEntity              entity);
        Task InsertBatchAsync<TEntity>(object partitionKey,  Func<TEntity, object> rowKeyGetter, IEnumerable<TEntity> entities);
    }

    // TODO: Include the URLs in the results (i.e. add NextPageUrl property) to the resources so the client doesn't have to assemble them.
    public class PagedResult<TEntity>
    {
        public List<TEntity>    Page            { get; set; }
        public string           NextPageToken   { get; set; }

        public PagedResult()
        {
            Page = new List<TEntity>();
        }
    }
}