using System;
using Ninject.Activation;
using TechTalk.SpecFlow;
using Votus.Testing.Integration.ApiClients.Votus.Models;
using Votus.Testing.Integration.WebsiteModels;
using Xunit;

namespace Votus.Testing.Integration.Acceptance.Steps
{
    [Binding]
    class TaskSteps : BaseSteps
    {
        private const string ValidTaskTitle = "This is a valid task";

        [When(@"a Voter submits a valid Task to the Idea")]
        public void WhenAVoterSubmitsAValidTaskToTheIdea()
        {
            var idea = ContextGet<Idea>();
            var task = ContextGet<HomePage>()
                .SubmitTask(
                    idea, 
                    ValidTaskTitle
                );

            ContextSet(task);
        }
        
        [Then(@"the Task appears under the Idea")]
        public void ThenTheTaskAppearsUnderTheIdea()
        {
            var idea    = ContextGet<Idea>();
            var newTask = ContextGet<Task>();

            var actualTask = ContextGet<HomePage>()
                .GetTaskFromIdeaList(idea, newTask);

            Assert.Equal(newTask, actualTask);
        }

        [When(@"a Voter submits an invalid Task to the Idea")]
        public void WhenAVoterSubmitsAnInvalidTaskToTheIdea()
        {
            ContextGet<HomePage>().SubmitInvalidTask(ContextGet<Idea>());
        }

        [Given(@"an Idea with a Task exists")]
        public void GivenAnIdeaWithATaskExists()
        {
            var createIdeaCommand = new CreateIdeaCommand();
            var createTaskCommand = new CreateTaskCommand(createIdeaCommand.NewIdeaId);

            VotusApiClient.Commands.Send(
                createIdeaCommand.NewIdeaId, 
                createIdeaCommand
            );

            ContextSet(
                VotusApiClient.Ideas.Get(createIdeaCommand.NewIdeaId)
            );

            VotusApiClient.Commands.Send(
                createTaskCommand.NewTaskId, 
                createTaskCommand
            );

            ContextSet(
                VotusApiClient.Tasks.Get(createTaskCommand.NewTaskId)
            );
        }

        [When(@"a Voter votes a Task is Completed")]
        public void WhenAVoterVotesATaskIsCompleted()
        {
            var idea = ContextGet<Idea>();
            var task = ContextGet<Task>();
            
            var homepage = Browser.NavigateToPage<HomePage>();

            homepage.VoteTaskIsCompleted(idea, task);

            ContextSet(homepage);
        }

        [Then(@"the Task Completed vote count is incremented")]
        public void ThenTheTaskCompletedVoteCountIsIncremented()
        {
            var idea                       = ContextGet<Idea>();
            var task                       = ContextGet<Task>();
            var previousCompletedVoteCount = ContextGet<Task>().CompletedVoteCount;
            var homepage                   = ContextGet<HomePage>();

            var currentCompletedVoteCount = homepage
                .Ideas[idea.Id]
                .Tasks[task.Id]
                .CompletedVoteCount;

            Assert.Equal(previousCompletedVoteCount + 1, currentCompletedVoteCount);
        } 
    }
}
