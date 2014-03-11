using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using Votus.Testing.Integration.Acceptance;
using Votus.Testing.Integration.ApiClients.Votus.Models;

namespace Votus.Testing.Integration.WebsiteModels
{
    class HomePage : BasePage
    {
        #region Constants

        public const string NoExcludeTag     = null;
        public const string RelativePath     = "/";
        public const string InvalidGoalTitle = "InvalidTitle";
        public const string InvalidTaskTitle = "InvalidTitle";
        public const string ValidIdeaTitle   = "Valid test idea";
        public const string VotusTestingTag  = "votus-test";

        #endregion

        #region HTML Elements

        [FindsBy] public IWebElement NewIdeaId                = null;
        [FindsBy] public IWebElement NewIdeaTitle             = null;
        [FindsBy] public IWebElement NewIdeaTag               = null;
        [FindsBy] public IWebElement Tags                     = null;
        [FindsBy] public IWebElement SubmitNewIdeaButton      = null;
        [FindsBy] public IWebElement TagButtonVotusTest       = null;
        [FindsBy] public IWebElement SystemVersionInfo        = null;
        [FindsBy] public IWebElement EnvironmentName          = null;
        [FindsBy] public IWebElement TagFilterLoadingIdeasIcon= null;

        #endregion

        private IdeaListPageSection _ideas;
        public IdeaListPageSection Ideas
        {
            get { return _ideas ?? (_ideas = new IdeaListPageSection(Browser)); }
        }

        public 
        HomePage() 
            : base(RelativePath)
        {
        }

        public
        void
        SubmitInvalidIdea()
        {
            NewIdeaTitle.SendKeys(SharedResources.TestId);
        }

        public
        IdeaPageSection
        SubmitIdea(
            string title    = ValidIdeaTitle,
            string tag      = VotusTestingTag)
        {
            var id = NewIdeaId.GetAttributeValue<Guid>(attributeName: "value"); 
            title  = string.Format("{0} {1}", title, SharedResources.TestId);

            NewIdeaTitle.SendKeys(title);
            NewIdeaTag.SendKeys(tag);

            SubmitNewIdeaButton.Click();

            return Ideas[id];
        }

        public 
        void
        ShowTestData()
        {
            var excludedTags = Tags.FindElements(By.ClassName("TagExcluded"));

            if (!excludedTags.Contains(TagButtonVotusTest)) return;

            // De-select the "votus-testing" tag button 
            TagButtonVotusTest.Click();

            new WebDriverWait(Browser, TimeSpan.FromSeconds(15))
                .Until(browser => !TagFilterLoadingIdeasIcon.Displayed);
        }

        public
        string
        GetCurrentErrorMessage()
        {
            return Browser.GetElementText(By.ClassName("field-validation-error"));
        }

        public 
        Goal
        SubmitGoal(
            Idea    idea, 
            string  goalTitle)
        {
            SendKeysToNewGoalTitle(idea, goalTitle);

            var ideaElement = Browser.GetElementById(idea.Id);

            var goalId = ideaElement
                .GetElementByClass("NewGoalId")
                .GetAttributeValue<Guid>("value");

            ideaElement
                .GetElementByClass("NewGoalButton")
                .Click();

            return new Goal {
                Id    = goalId,
                Title = goalTitle
            };
        }

        public 
        Task 
        SubmitTask(
            Idea    idea, 
            string  taskTitle)
        {
            SendKeysToNewTaskTitle(idea, taskTitle);

            var ideaElement = Browser.GetElementById(idea.Id);

            var taskId = ideaElement
                .GetElementByClass("NewTaskId")
                .GetAttributeValue<Guid>("value");

            ideaElement
                .GetElementByClass("NewTaskButton")
                .Click();

            return new Task {
                Id    = taskId,
                Title = taskTitle
            };
        }

        public 
        void
        SendKeysToNewGoalTitle(
            Idea    idea, 
            string  newGoalTitle)
        {
            ShowIdeaDetails(idea);
            Ideas[idea.Id].ShowGoalsDisplay();

            Browser.GetElementById(idea.Id)
                .GetElementByClass("NewGoalTitle")
                .SendKeys(newGoalTitle);
        }

        public 
        void
        SendKeysToNewTaskTitle(
            Idea    idea, 
            string  newTaskTitle)
        {
            ShowIdeaDetails(idea);
            Ideas[idea.Id].ShowTasksDisplay();

            Browser.GetElementById(idea.Id)
                .GetElementByClass("NewTaskTitle")
                .SendKeys(newTaskTitle);
        }

        public
        void
        ShowIdeaDetails(
            Idea idea)
        {
            var ideaElement = Browser
                .GetElementById(idea.Id);

            if (!ideaElement.FindElement(By.ClassName("IdeaBody")).Displayed)
                ideaElement
                    .GetElementByClass("IdeaHeader")
                    .Click();
        }

        public 
        void
        SubmitInvalidGoal(
            Idea idea)
        {
            SendKeysToNewGoalTitle(idea, InvalidGoalTitle);
        }

        public
        void
        SubmitInvalidTask(
            Idea idea)
        {
            SendKeysToNewTaskTitle(idea, InvalidTaskTitle);
        }

        private
        static
        Task
        ConvertToTaskModel(
            IWebElement taskElement)
        {
            return new Task {
                Id    = taskElement.GetAttributeValue<Guid>("Id"),
                Title = taskElement.GetElementByClass("Title").Text
            };
        }

        public
        Task
        GetTaskFromIdeaList(
            Idea    idea,
            Task    task)
        {
            ShowIdeaDetails(idea);

            return ConvertToTaskModel(
                Browser.FindElement(By.Id(task.Id.ToString()))
            );
        }

        public 
        Version 
        GetSystemVersionNumber()
        {
            return new Version(SystemVersionInfo.Text.TrimStart('v'));
        }

        public 
        string 
        GetEnvironmentName()
        {
            return EnvironmentName.Text;
        }

        public 
        void 
        VoteTaskIsCompleted(
            Idea idea, // TODO: Switch to using PageModels instead of the API models.
            Task task)
        {
            ShowIdeaDetails(idea);
            Ideas[idea.Id].ShowTasksDisplay();

            Browser.GetElementById(task.Id)
                .GetElementByClass("VoteCompletedButton")
                .Click();

            // TODO: Wait for success/fail
        }
    }
}