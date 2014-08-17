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
            var ideaId = VotusApiClient.Ideas.Create();

            ContextSet(
                VotusApiClient.Ideas.Get(ideaId)
            );

            var createTaskCommand = new CreateTaskCommand(ideaId);

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
            
            var taskPageSection = ContextGet<HomePage>()
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

        [Given(@"a Voter has previously voted a Task is Completed")]
        public void GivenAVoterHasPreviouslyVotedATaskIsCompleted()
        {
            GivenAnIdeaWithATaskExists();

            var ideaWithATask = ContextGet<Task>();

            VotusApiClient.Tasks[ideaWithATask.Id].VoteCompleted();
        }

        [Then(@"the Voter cannot vote the Task is Completed again")]
        public void ThenTheVoterCannotVoteTheTaskIsCompletedAgain()
        {
            WhenAVoterVotesATaskIsCompleted();

            Assert.Equal(1, ContextGet<TaskPageSection>().CompletedVoteCount);
        }

    }
}
