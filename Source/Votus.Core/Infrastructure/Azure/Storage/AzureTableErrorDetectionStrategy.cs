using System;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Votus.Core.Infrastructure.Azure.Storage
{
    class AzureTableErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public CloudTable Table { get; set; }

        public 
            AzureTableErrorDetectionStrategy(
            CloudTable table)
        {
            Table = table;
        }

        public
            bool 
            IsTransient(
            Exception ex)
        {
            var storageException = ex as StorageException;

            return storageException != null 
                   && StorageExceptionIsTransient(storageException);
        }

        private
            bool
            StorageExceptionIsTransient(
            StorageException storageException)
        {
            var storageErrorCode = storageException
                .RequestInformation
                .ExtendedErrorInformation
                .ErrorCode;

            var resolved = ResolveError(storageErrorCode);

            // The error can be considered transient if it has been resolved.
            return resolved;
        }

        private
            bool
            ResolveError(
            string storageErrorCode)
        {
            switch (storageErrorCode)
            {
                case "TableNotFound":
                    Table.CreateIfNotExists();
                    return true;
                default:
                    return false;
            }
        }
    }
}