using System;
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
                .Ideas[idea.Id]
                .ShowTasksDisplay()
                .SubmitTask(ValidTaskTitle);

            ContextSet(task);
        }
        
        [Then(@"the Task appears under the Idea")]
        public void ThenTheTaskAppearsUnderTheIdea()
        {
            var idea    = ContextGet<Idea>();
            var newTask = ContextGet<TaskPageSection>();

            var actualTask = ContextGet<HomePage>()
                .Ideas[idea.Id]
                .Tasks[newTask.Id];

            Assert.NotNull(actualTask);
        }

        [When(@"a Voter submits an invalid Task to the Idea")]
        public void WhenAVoterSubmitsAnInvalidTaskToTheIdea()
        {
            var invalidTaskTitle = "invalidtask";
            var idea             = ContextGet<Idea>();

            try
            {
                ContextGet<HomePage>()
                    .Ideas[idea.Id]
                    .ShowTasksDisplay()
                    .SubmitTask(invalidTaskTitle);
            }
            catch (Exception ex)
            {
                ContextSet(ex);
            }
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
            var apiIdea = ContextGet<Idea>();
            var apiTask = ContextGet<Task>();
            
            var taskPageSection = Browser.NavigateToPage<HomePage>()
                .Ideas[apiIdea.Id]
                .ShowTasksDisplay()[apiTask.Id]
                .VoteCompleted();

            ContextSet(taskPageSection);
        }

        [Then(@"the Task Completed vote count is incremented")]
        public void ThenTheTaskCompletedVoteCountIsIncremented()
        {
            var apiTask         = ContextGet<Task>();
            var taskPageSection = ContextGet<TaskPageSection>();

            var previousCompletedVoteCount = apiTask.CompletedVoteCount;
            var currentCompletedVoteCount  = taskPageSection.CompletedVoteCount;

            Assert.Equal(
                previousCompletedVoteCount + 1, 
                currentCompletedVoteCount
            );
        } 
    }
}
