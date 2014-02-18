using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Votus.Core.Infrastructure.Azure.Storage
{
    public static class BlobExtensions
    {
        public
        static
        async Task
        UploadFromStringAsync(
            this
            CloudBlockBlob  blob,
            string          str)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(str))) 
                await blob.UploadFromStreamAsync(stream);
        }

        public
        static
        async Task<string>
        DownloadToStringAsync(this CloudBlockBlob blob)
        {
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);

                stream.Position = 0;

                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
