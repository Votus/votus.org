using System.Net.Http;

namespace Votus.Core.Infrastructure.Net.Http
{
    public static class Extensions
    {
        public 
        static 
        bool 
        HasContent(
            this 
            HttpResponseMessage response)
        {
            var objectContent = response.Content as ObjectContent;

            if (objectContent != null)
                return objectContent.Value != null;

            var byteArrayContent = response.Content as ByteArrayContent;

            return byteArrayContent != null;
        }
    }
}
