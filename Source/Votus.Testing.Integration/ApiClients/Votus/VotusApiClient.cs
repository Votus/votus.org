using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Votus.Core.Infrastructure.Serialization;
using Votus.Core.Infrastructure.Web;
using Votus.Testing.Integration.ApiClients.Votus.Models;

namespace Votus.Testing.Integration.ApiClients.Votus
{
    internal class VotusApiClient
    {


        public IdeaApiEntity Ideas;
        public TaskApiEntity Tasks;
        public CommandApi    Commands;

        [Inject] public ISerializer Serializer { get; set; }
        [Inject] public IHttpClient HttpClient { get; set; }

        public VotusApiClient()
        {
            Commands = new CommandApi(this);
            Ideas    = new IdeaApiEntity(this);
            Tasks    = new TaskApiEntity(this);
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
            async System.Threading.Tasks.Task SendAsync(
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
                int     pollForSeconds           = 60,
                int     pollIntervalMilliseconds = 500)
            {
                var stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    var url = string.Format("/api/ideas/{0}", ideaId);

                    Idea idea = null;

                    try
                    {
                        idea = _baseApiClient
                            .HttpClient
                            .Get<Idea>(url)
                            .Payload;
                    }
                    catch (RequestFailedException requestFailedException)
                    {
                        // Allow 404 Not Found to be treated simply as null, but throw on anything else...
                        if (requestFailedException.Response.StatusCode != HttpStatusCode.NotFound)
                            throw;
                    }

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

                    Thread.Sleep(
                        TimeSpan.FromMilliseconds(pollIntervalMilliseconds)
                    );
                }
            }
        }

        internal class TaskApiEntity
        {
            private readonly VotusApiClient _baseApiClient;

            public
            TaskApiEntity(
                VotusApiClient baseApiClient)
            {
                _baseApiClient = baseApiClient;
            }            

            // TODO: Consolidate polling logic further down in the HttpClient.

            public 
            Task 
            Get(
                Guid    taskId,
                int     pollForSeconds = 60)
            {
                var stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    var url = string.Format("/api/tasks/{0}", taskId);

                    Task task = null;

                    try
                    {
                        task = _baseApiClient
                            .HttpClient
                            .Get<Task>(url)
                            .Payload;
                    }
                    catch (RequestFailedException requestFailedException)
                    {
                        // Allow 404 Not Found to be treated simply as null, but throw on anything else...
                        if (requestFailedException.Response.StatusCode != HttpStatusCode.NotFound)
                            throw;
                    }

                    if (task != null)
                        return task;

                    if (stopwatch.Elapsed.TotalSeconds > pollForSeconds)
                        throw new Exception(
                            string.Format(
                                "Could not find task {0} after {1} seconds.", 
                                taskId, 
                                pollForSeconds
                            )
                        );
                }
            }
        }
    }
}