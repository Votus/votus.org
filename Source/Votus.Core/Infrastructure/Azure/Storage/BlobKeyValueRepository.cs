using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using System.Threading.Tasks;
using Ninject;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Azure.Storage
{
    public class BlobKeyValueRepository : IKeyValueRepository
    {
        public string ContainerName { get; set; }

        [Inject] public ISerializer         Serializer          { get; set; }
        [Inject] public CloudStorageAccount CloudStorageAccount { get; set; }

        private CloudBlobContainer _blobContainer;
        private CloudBlobContainer BlobContainer
        {
            get
            {
                if (_blobContainer == null)
                {
                    _blobContainer = CloudStorageAccount.CreateCloudBlobClient().GetContainerReference(ContainerName);
                    _blobContainer.CreateIfNotExists();
                }

                return _blobContainer;
            }
        }

        public
        BlobKeyValueRepository(string containerName)
        {
            ContainerName = containerName;
        }

        public
        async Task
        SetAsync<TValue>(
            string key, 
            TValue value)
        {
            var blob            = BlobContainer.GetBlockBlobReference(key);
            var serializedValue = Serializer.Serialize(value);

            await blob.UploadFromStringAsync(serializedValue);
        }

        public
        async Task<TValue>
        GetAsync<TValue>(string key)
        {
            var blob = BlobContainer.GetBlockBlobReference(key);

            try
            {
                var serializedValue = await blob.DownloadToStringAsync();

                // No need to deserialize if the TValue type is primitive, such as a string.
                if (typeof(TValue) == typeof(string))
                    return (TValue)(object)serializedValue;

                return Serializer.Deserialize<TValue>(serializedValue);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    return default(TValue);

                throw;
            }
        }
    }
}
