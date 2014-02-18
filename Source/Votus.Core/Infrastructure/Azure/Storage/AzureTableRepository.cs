using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage.Table;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Azure.Storage
{
    public class AzureTableRepository : IPartitionedRepository
    {
        #region Variables & Properties

        [Inject] public ISerializer Serializer { get; set; }

        public CloudTable   Table                   { get; private set; }
        public RetryPolicy  TableStorageRetryPolicy { get; private set; }

        #endregion

        #region Constructors

        public 
        AzureTableRepository(
            CloudTable table)
        {
            Table                   = table;
            TableStorageRetryPolicy = new RetryPolicy(
                new AzureTableErrorDetectionStrategy(Table),
                new ExponentialBackoff()
            );
        }

        #endregion

        #region IPertitionedRepository Members

        public 
        async Task<IEnumerable<TEntity>>
        GetPartitionAsync<TEntity>(
            object partitionKey)
        {
            var query = new TableQuery()
                .Where(
                    TableQuery.GenerateFilterCondition(
                        "PartitionKey", 
                        QueryComparisons.Equal, 
                        partitionKey.ToString()
                    )
                );

            var allResults = new List<TEntity>();

            TableContinuationToken token = null;

            do
            {
                var response = await ExecuteTableQueryAsync(
                    query,
                    token
                );

                token = response.ContinuationToken;

                allResults.AddRange(
                    response.Results.Select(ConvertToEntity<TEntity>)
                );

            } while (token != null);

            return allResults;
        }

        public 
        Task 
        InsertAsync<TEntity>(
            object  partitionKey, 
            object  rowKey, 
            TEntity entity)
        {
            var tableEntity = ConvertToTableEntity(
                partitionKey, 
                rowKey, 
                entity
            );

            var operation = TableOperation.Insert(tableEntity);

            return TableStorageRetryPolicy.ExecuteAsync(() => 
                Table.ExecuteAsync(operation)
            );
        }

        public
        async Task
        InsertBatchAsync<TEntity>(
            object                  partitionKey,
            Func<TEntity, object>   rowKeyGetter,
            IEnumerable<TEntity>    entities)
        {
            var op = new TableBatchOperation();
            
            foreach (var entity in entities)
            {
                var rowKey             = rowKeyGetter(entity);
                var tableEntity        = ConvertToTableEntity(partitionKey, rowKey, entity);

                op.Add(TableOperation.Insert(tableEntity));
            }

            await TableStorageRetryPolicy.ExecuteAsync(() => 
                Table.ExecuteBatchAsync(op)
            );
        }

        public
        async Task<IEnumerable<TEntity>>
        GetAllAsync<TEntity>()
        {
            var allResults = new List<TEntity>();
            var query      = new TableQuery();

            TableContinuationToken token = null;

            do
            {
                var response = await ExecuteTableQueryAsync(
                    query,
                    token
                );

                token = response.ContinuationToken;

                allResults.AddRange(
                    response.Results.Select(ConvertToEntity<TEntity>)
                );

            } while (token != null);

            return allResults;
        }

        public
        async Task<PagedResult<TEntity>>
        GetAllPagedAsync<TEntity>(
            string  nextPageToken,
            int     maxPerPage)
        {
            var query = new TableQuery()
                .Take(maxPerPage);

            TableContinuationToken token = null;

            if (nextPageToken != null && nextPageToken != "null")
            {
                var tokenParts = nextPageToken.Split(new[] {"__"}, StringSplitOptions.RemoveEmptyEntries);

                var nextPartition = tokenParts[0];
                var nextRow       = tokenParts[1];

                token = new TableContinuationToken {
                    NextPartitionKey = nextPartition,
                    NextRowKey       = nextRow
                };
            }

            var response = await ExecuteTableQueryAsync(
                query, 
                token
            );

            string continuationToken = null;
            

            if (response.ContinuationToken != null) {
                token             = response.ContinuationToken;
                continuationToken = string.Format("{0}__{1}", token.NextPartitionKey, token.NextRowKey);
            }

            return new PagedResult<TEntity> {
                Page          = response.Results.Select(ConvertToEntity<TEntity>).ToList(),
                NextPageToken = continuationToken
            };
        }

        #endregion

        #region Methods

        private 
        Task<TableQuerySegment<DynamicTableEntity>>
        ExecuteTableQueryAsync(
            TableQuery              query, 
            TableContinuationToken  token)

        {
            return TableStorageRetryPolicy.ExecuteAsync(() =>
                Table.ExecuteQuerySegmentedAsync(
                    query: query,
                    token: token
                )
            );
        }

        private
        ITableEntity
        ConvertToTableEntity(
            object partitionKey, 
            object rowKey, 
            object entity)
        {
            var serializedEntity = Serializer.Serialize(entity);
            
            return new DynamicTableEntity(partitionKey.ToString(), rowKey.ToString()) {
                Properties = { {"Payload", new EntityProperty(serializedEntity)} }
            };
        }

        private 
        TEntity 
        ConvertToEntity<TEntity>(
            DynamicTableEntity dynamicTableEntity)
        {
            var serializedEntity = dynamicTableEntity["Payload"].StringValue;
            
            return Serializer.Deserialize<TEntity>(serializedEntity);
        }

        #endregion
    }
}