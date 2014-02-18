using System.Collections.Generic;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class PagedResult<T>
    {
        public IEnumerable<T>   Page            { get; set; }
        public string           NextPageToken   { get; set; }
    }
}
