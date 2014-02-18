using Ninject;
using System.Collections.Generic;
using System.Linq;
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
        private const char TokenDelimiter = ':';

        [Inject] public QueueManager            CommandDispatcher   { get; set; }
        [Inject] public IPartitionedRepository  IdeasRepository     { get; set; }
        
        [Route("ideas")]
        public
        async Task<PagedResult<IdeaViewModel>>
        GetIdeasAsync(
            string nextPageToken = null,
            string excludeTag    = null,
            int    itemsPerPage  = 10)
        {
            // TODO: Break up / simplify this method...

            // Initialize variable to collect all the results.
            var allResults = new PagedResult<IdeaViewModel> {
                NextPageToken = nextPageToken
            };
            
            // Loop over pages from the repo to produce complete 'logical' pages.
            do
            {
                // Split controller token from repo token.
                var pagingTokens = allResults.NextPageToken == null ? 
                    new string[] { } : allResults.NextPageToken.Split(TokenDelimiter);

                var repoToken       = pagingTokens.FirstOrDefault();
                var controllerToken = pagingTokens.LastOrDefault();

                // Attempt to parse the controller token in to the skip parameter if possible...
                var skip = 0;
                int.TryParse(controllerToken, out skip);

                // Calculate the number of filtered results to take from the next results...
                var take = itemsPerPage - allResults.Page.Count;

                // Get a page of data from the repo.
                var currentPage = await IdeasRepository
                    .GetAllPagedAsync<IdeaViewModel>(
                        repoToken, 
                        itemsPerPage
                    );

                // Filter the data as specified by the API request.
                var filteredPage = FilterIdeasByTag(currentPage.Page, excludeTag);
                var resultsToAdd = filteredPage.Skip(skip).Take(take).ToArray();

                // Update the results with the latest repo page.
                allResults.Page.AddRange(resultsToAdd);

                var totalRepoResultsAdded = skip + resultsToAdd.Count();

                // Determine if all the results from the current page have been used.
                if (totalRepoResultsAdded < filteredPage.Count())
                {
                    // If items remaining, re-use last page token
                    repoToken = allResults.NextPageToken;
                    skip      = totalRepoResultsAdded;
                }
                else
                {
                    // Switch to the next page...
                    repoToken = currentPage.NextPageToken;
                    skip      = 0;
                }
                
                // Create the next page token.
                allResults.NextPageToken = AssembleNextPageControllerToken(
                    repoToken, 
                    skip, 
                    TokenDelimiter
                );

            } while (allResults.Page.Count() < itemsPerPage && allResults.NextPageToken != null);

            // Return the results.
            return allResults;
        }

        public 
        static 
        List<IdeaViewModel>
        FilterIdeasByTag(
            List<IdeaViewModel> ideas, 
            string              tag)
        {
            return string.IsNullOrWhiteSpace(tag) ?
                ideas : ideas.Where(idea => idea.Tag != tag).ToList();
        }

        public 
        static 
        string 
        AssembleNextPageControllerToken(
            string  repoToken, 
            int     skip, 
            char    delimiter)
        {
            if (skip == 0) return repoToken;

            return repoToken == null ?
                null : string.Format(
                    "{0}{1}{2}",
                    repoToken,
                    TokenDelimiter,
                    skip
                );
        }
    }
}