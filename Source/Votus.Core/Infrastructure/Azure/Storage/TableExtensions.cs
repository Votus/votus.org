using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.Azure.Storage
{
    public static class TableExtensions
    {
        public 
        static
        async Task 
        ExecuteAsync(
            this
            CloudTable      table,
            TableOperation  operation
            )
        {
            try { 
                await Task.Factory.FromAsync(
                    beginMethod:    (callback, state) => table.BeginExecute(operation, callback, state),
                    endMethod:      (result         ) => table.EndExecute(result),
                    state:          null
                );
            }
            catch(StorageException storageException)
            {
                if (storageException.RequestInformation.HttpStatusCode != (int) HttpStatusCode.NotFound)
                    throw;
            }

            // Create the table...
            await table.CreateIfNotExistsAsync();

            // Try again..
            await table.ExecuteAsync(operation);
        }
    }
}
