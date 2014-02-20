using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Votus.Core.Infrastructure.Serialization;
using Votus.Core.Infrastructure.Web;
using Votus.Testing.Integration.ApiClients.Votus.Models;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Integration.ApiClients.Votus
{
    internal class VotusApiClient
    {


        public IdeaApiEntity Ideas;
        public CommandApi    Commands;

        [Inject] public ISerializer Serializer { get; set; }
        [Inject] public IHttpClient HttpClient { get; set; }

        public VotusApiClient()
        {
            Commands = new CommandApi(this);
            Ideas    = new IdeaApiEntity(this);
        }

        internal class CommandApi
        {
            private readonly VotusApiClient _baseApiClient;

            public CommandApi(VotusApiClient baseApiClient)
            {
                _baseApiClient = baseApiClient;
            }

            public
            void
            Send(
                Guid    commandId, 
                object  command)
            {
                Send(commandId, new CommandEnvelope(command));
            }

            public
            void
            Send(
                Guid            commandId,
                CommandEnvelope commandEnvelope)
            {
                try
                {
                    _baseApiClient.HttpClient.Put(
                        string.Format("/api/commands/{0}", commandId),
                        commandEnvelope
                        );
                }
                catch (RequestFailedException requestFailedException)
                {
                    throw new VotusApiException(
                        _baseApiClient.Serializer,
                        requestFailedException
                    );
                }
            }

            public 
            async Task 
            SendAsync(
                object command)
            {
                var commandId       = Guid.NewGuid();
                var commandEnvelope = new CommandEnvelope(command);
                
                try
                {
                    await _baseApiClient.HttpClient.PutAsync(
                        string.Format("/api/commands/{0}", commandId),
                        commandEnvelope
                        );
                }
                catch (RequestFailedException requestFailedException)
                {
                    throw new VotusApiException(
                        _baseApiClient.Serializer,
                        requestFailedException
                    );
                }
            }
        }

        internal class IdeaApiEntity
        {
            private readonly VotusApiClient _baseApiClient;

            public
            IdeaApiEntity(VotusApiClient baseApiClient)
            {
                _baseApiClient = baseApiClient;
            }

            public
            PagedResult<Idea>
            GetPage(
                string nextPageToken = null
                )
            {
                var url = "/api/ideas";

                if (nextPageToken != null)
                    url += "?nextPageToken=" + nextPageToken;

                return _baseApiClient
                    .HttpClient
                    .Get<PagedResult<Idea>>(url)
                    .Payload;
            }

            public
            IEnumerable<Idea>
            GetAllDescending()
            {
                string nextPageToken = null;

                do
                {
                    var pagedResult = GetPage(nextPageToken: nextPageToken);
                    nextPageToken   = pagedResult.NextPageToken;

                    foreach (var idea in pagedResult.Page)
                        yield return idea;

                } while (nextPageToken != null);
            }
        

            public 
            Idea 
            Get(
                Guid    ideaId,
                int     pollForSeconds = 60)
            {
                var stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    // TODO: Implement an API to get an Idea by id so we don't have to iterate over all ideas to find it.
                    // Alternatively, add timestamps to the ideas and only page back far enough in time as needed.

                    var idea = _baseApiClient
                        .Ideas
                        .GetAllDescending()
                        .SingleOrDefault(i => i.Id == ideaId);

                    if (idea != null)
                        return idea;

                    if (stopwatch.Elapsed.TotalSeconds > pollForSeconds)
                        throw new Exception(
                            string.Format(
                                "Could not find idea {0} after {1} seconds.", 
                                ideaId, 
                                pollForSeconds
                            )
                        );
                }
            }
        }
    }
}