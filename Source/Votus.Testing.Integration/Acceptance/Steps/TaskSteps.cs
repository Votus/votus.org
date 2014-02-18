using TechTalk.SpecFlow;
using Votus.Testing.Integration.Acceptance.Pages;
using Votus.Testing.Integration.ApiClients.Votus.Models;
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
    }
}
