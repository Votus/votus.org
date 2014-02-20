using Ninject;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Votus.Web.Areas.Api.Models;

namespace Votus.Web.Areas.Api.Controllers
{
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName)]
    public class IdeasController : ApiController
    {
        [Inject] public QueueManager            CommandDispatcher   { get; set; }
        [Inject] public IPartitionedRepository  IdeasRepository     { get; set; }

        [Route("ideas")]
        public
        Task<PagedResult<IdeaViewModel>>
        GetIdeasAsync(
            string  nextPageToken = null,
            string  excludeTag    = null,
            int     itemsPerPage  = 10)
        {
            return IdeasRepository.GetWherePagedAsync<IdeaViewModel>(
                idea => idea.Tag != excludeTag,
                nextPageToken,
                itemsPerPage
            );
        }
    }
}