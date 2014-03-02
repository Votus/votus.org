using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Votus.Testing.Integration.ApiClients.Votus.Models;

namespace Votus.Testing.Integration.Acceptance.Pages
{
    class HomePage : BasePage
    {
        public const string NoExcludeTag     = null;
        public const string RelativePath     = "/";
        public const string InvalidGoalTitle = "InvalidTitle";
        public const string InvalidTaskTitle = "InvalidTitle";
        public const string ValidIdeaTitle   = "Valid test idea";
        public const string VotusTestingTag  = "votus-test";

        // HTML Elements
        [FindsBy] public IWebElement NewIdeaTitle             = null;
        [FindsBy] public IWebElement NewIdeaId                = null;
        [FindsBy] public IWebElement Tag                      = null;
        [FindsBy] public IWebElement Tags                     = null;
        [FindsBy] public IWebElement SubmitNewIdeaButton      = null;
        [FindsBy] public IWebElement TagButtonVotusTest       = null;
        [FindsBy] public IWebElement SystemVersionInfo        = null;
        [FindsBy] public IWebElement EnvironmentName          = null;
        [FindsBy] public IWebElement TagFilterLoadingIdeasIcon= null;
        
        // Page Sections
        private IdeasSection _ideas;
        public IdeasSection Ideas 
        { 
            get
            {
                // TODO: Use Ninject to inject this?
                if (_ideas != null) return _ideas;

                _ideas = new IdeasSection { Browser = Browser };

                PageFactory.InitElements(Browser, _ideas);

                return _ideas;
            }
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
        Idea
        SubmitIdea(
            string title    = ValidIdeaTitle,
            string tag      = VotusTestingTag)
        {
            var id = NewIdeaId.GetAttributeValue<Guid>(attributeName: "value"); 
            title  = string.Format("{0} {1}", title, SharedResources.TestId);

            NewIdeaTitle.SendKeys(title);
            Tag.SendKeys(tag);

            SubmitNewIdeaButton.Click();
            
            return new Idea {
                Id    = id,
                Title = title,
                Tag   = tag
            };
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
                .GetAttributeValue("value");

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
                .GetAttributeValue("value");

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
            ShowGoalsDisplay(idea);

            Browser.GetElementById(idea.Id)
                .GetElementByClass("NewGoalTitle")
                .SendKeys(newGoalTitle);
        }

        private 
        void 
        ShowGoalsDisplay(
            Idea idea)
        {
            var ideaElement = Browser
                .GetElementById(idea.Id);

            if (!ideaElement.FindElement(By.ClassName("GoalsDisplay")).Displayed)
                ideaElement
                    .GetElementByClass("GoalsHeader")
                    .Click();
        }

        public 
        void
        SendKeysToNewTaskTitle(
            Idea    idea, 
            string  newTaskTitle)
        {
            ShowIdeaDetails(idea);
            ShowTasksDisplay(idea);

            Browser.GetElementById(idea.Id)
                .GetElementByClass("NewTaskTitle")
                .SendKeys(newTaskTitle);
        }

        private 
        void 
        ShowTasksDisplay(
            Idea idea)
        {
            var ideaElement = Browser
                .GetElementById(idea.Id);

            if (!ideaElement.FindElement(By.ClassName("TasksDisplay")).Displayed)
                ideaElement
                    .GetElementByClass("TasksHeader")
                    .Click();
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

        public
        IEnumerable<Goal>
        GetGoalsList(Idea idea)
        {
            ShowIdeaDetails(idea);

            var ideaElement  = Browser.GetElementById(idea.Id);
            var goalsElement = ideaElement.GetElementByClass("Goals");

            return goalsElement.FindElements(By.ClassName("Goal"))
                .Select(ConvertToGoalModel);
        }

        private 
        static 
        Goal
        ConvertToGoalModel(IWebElement goalElement)
        {
            return new Goal {
                Id    = goalElement.GetAttributeValue("Id"),
                Title = goalElement.GetElementByClass("Title").Text
            };
        }

        private
        static
        Task
        ConvertToTaskModel(
            IWebElement taskElement)
        {
            return new Task {
                Id    = taskElement.GetAttributeValue("Id"),
                Title = taskElement.GetElementByClass("Title").Text
            };
        }

        public 
        Goal 
        GetGoalFromIdeaList(
            Idea    idea,
            Goal    goal)
        {
            ShowIdeaDetails(idea);

            return ConvertToGoalModel(
                Browser.FindElement(By.Id(goal.Id))
            );
        }

        public
        Task
        GetTaskFromIdeaList(
            Idea    idea,
            Task    task)
        {
            ShowIdeaDetails(idea);

            return ConvertToTaskModel(
                Browser.FindElement(By.Id(task.Id))
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

        public class IdeasSection
        {
            public IWebDriver Browser { get; set; }

            [FindsBy]
            public IWebElement LoadNextIdeasButton { get; set; }

            public
            Idea
            GetIdeaFromList(
                Guid id)
            {
                return ConvertToModel(id);
            }

            public
            IEnumerable<Idea>
            GetIdeasList()
            {
                return Browser
                    .FindElements(By.CssSelector("#Ideas .Idea"))
                    .Select(e => e.GetAttributeValue<Guid>("Id"))
                    .Select(ConvertToModel);
            }

            private 
            Idea
            ConvertToModel(
                Guid ideaId)
            {
                var ideaElement = Browser.GetElementById(ideaId);
            
                return new Idea {
                    Id    = ideaId,
                    Title = ideaElement.GetElementByClass("Title").Text,
                    Tag   = ideaElement.GetElementByClass("Tag").Text
                };
            }

            public 
            IEnumerable<Idea>
            GetAll()
            {
                bool morePages;

                do
                    morePages = LoadNextPage();
                while (morePages);

                return GetIdeasList();
            }

            public 
            bool 
            LoadNextPage()
            {
                if (LoadNextIdeasButton.Displayed)
                    LoadNextIdeasButton.Click();

                // The button should first become disabled...
                Browser.WaitUntil(driver =>
                    !LoadNextIdeasButton.Enabled
                );

                // TODO: Detect errors, throw exception...
                
                // Then become either:
                // Enabled:       If the results came back and there is another page or,
                // Not Displayed: If the results came back and there is not another page
                Browser.WaitUntil(driver =>
                    LoadNextIdeasButton.Enabled || LoadNextIdeasButton.Displayed == false
                );

                return LoadNextIdeasButton.Displayed;
            }
        }
    }
}