using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Serialization;

namespace Votus.Core.Infrastructure.Azure.Storage
{
    public class BlobConfigProvider : IReadWriteRepository
    {
        private readonly Dictionary<string, string>  _cachedConfig;
        private readonly ISerializer                 _serializer;
        private readonly CloudBlobClient             _blobClient;

        public
        BlobConfigProvider(
            ISerializer         serializer,
            CloudStorageAccount storageAccount)
        {
            _serializer     = serializer;
            _blobClient     = storageAccount.CreateCloudBlobClient();
            _cachedConfig   = GetConfig(storageAccount);
        }

        private Dictionary<string, string> GetConfig(
            CloudStorageAccount storageAccount)
        {
            var blob = GetConfigBlob();

            if (!blob.Exists())
                throw new Exception(
                    string.Format(
                        "Could not find blob {0}{1}/{2}.  You may need to create and upload this file.",
                        storageAccount.BlobEndpoint,
                        blob.Container.Name,
                        blob.Name
                        )
                    );

            using (var memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);

                return _serializer.Deserialize<Dictionary<string, string>>(
                    memoryStream
                );
            }
        }

        private CloudBlockBlob GetConfigBlob()
        {
            var blobContainer = _blobClient.GetContainerReference("environment-configuration");
            return blobContainer.GetBlockBlobReference("settings.json");   
        }

        public string Get(string settingName)
        {
            return _cachedConfig.ContainsKey(settingName) ?
                _cachedConfig[settingName] : null;
        }

        public void Set(string settingName, string value)
        {
            var blob = GetConfigBlob();

            _cachedConfig[settingName] = value;

            var serializedConfig = _serializer.Serialize(_cachedConfig);

            using (var writeStream = blob.OpenWrite())
                new StreamWriter(writeStream) { AutoFlush = true }
                    .Write(serializedConfig);
        }
    }
}
