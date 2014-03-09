using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Votus.Testing.Integration.ApiClients.Votus.Models;

namespace Votus.Testing.Integration.Acceptance.Pages
{
    // TODO: Reorganize the Page models!

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
                // TODO: Use Ninject to inject this?  Might also just be able to use [FindsBy] on collections too
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
        IdeaSection
        SubmitIdea(
            string title    = ValidIdeaTitle,
            string tag      = VotusTestingTag)
        {
            var id = NewIdeaId.GetAttributeValue<Guid>(attributeName: "value"); 
            title  = string.Format("{0} {1}", title, SharedResources.TestId);

            NewIdeaTitle.SendKeys(title);
            Tag.SendKeys(tag);

            SubmitNewIdeaButton.Click();
            
            return new IdeaSection {
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
                Id    = taskElement.GetAttributeValue<Guid>("Id"),
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

        public class IdeasSection
        {
            public IWebDriver Browser { get; set; }

            [FindsBy]
            public IWebElement LoadNextIdeasButton { get; set; }

            public
            IdeaSection
            GetIdeaFromList(
                Guid id)
            {
                return ConvertToModel(id);
            }

            public
            IEnumerable<IdeaSection>
            GetIdeasList()
            {
                return Browser
                    .FindElements(By.CssSelector("#Ideas .Idea"))
                    .Select(e => e.GetAttributeValue<Guid>("Id"))
                    .Select(ConvertToModel);
            }

            private 
            IdeaSection
            ConvertToModel(
                Guid ideaId)
            {
                var ideaElement = Browser.GetElementById(ideaId);
            
                return new IdeaSection {
                    Id      = ideaId,
                    Title   = ideaElement.GetElementByClass("Title").Text,
                    Tag     = ideaElement.GetElementByClass("Tag").Text,
                    Browser = Browser
                };
            }

            public 
            IEnumerable<IdeaSection>
            GetAllDescending()
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
                if (!LoadNextIdeasButton.Displayed) return false;
                
                LoadNextIdeasButton.Click();
                
                // TODO: Detect errors, throw exception...
                
                // Then become either:
                // Enabled:       If the results came back and there is another page or,
                // Not Displayed: If the results came back and there is not another page
                Browser.WaitUntil(driver => {
                    var button = driver.FindElement(By.Id("LoadNextIdeasButton"));
                    
                    return button.Enabled || button.Displayed == false;
                });

                return LoadNextIdeasButton.Displayed;
            }

            public IdeaSection this[Guid ideaId]
            {
                get
                {
                    return GetIdeaFromList(ideaId);
                }
            }
        }

        public 
        void 
        VoteTaskIsCompleted(
            Idea idea, // TODO: Switch to using PageModels instead of the API models.
            Task task)
        {
            ShowIdeaDetails(idea);
            ShowTasksDisplay(idea);

            Browser.GetElementById(task.Id)
                .GetElementByClass("VoteCompletedButton")
                .Click();

            // TODO: Wait for success/fail
        }
    }

    public class IdeaSection
    {
        public Guid         Id      { get; set; }
        public string       Title   { get; set; }
        public string       Tag     { get; set; }
        public IWebDriver   Browser { get; set; }

        private TasksSection _tasks;

        public TasksSection Tasks
        {
            get
            {
                // TODO: Use Ninject to inject this?
                if (_tasks != null) return _tasks;

                _tasks = new TasksSection { Browser = Browser };

                PageFactory.InitElements(Browser, _tasks);

                return _tasks;
            }
        }

        #region ReSharper Generated Methods

        protected bool Equals(IdeaSection other)
        {
            return Id.Equals(other.Id) && string.Equals(Title, other.Title) && string.Equals(Tag, other.Tag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ Title.GetHashCode();
                hashCode = (hashCode*397) ^ Tag.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdeaSection) obj);
        }

        #endregion
    }

    public class TasksSection
    {
        public TaskSection this[Guid taskId]
        {
            get { return ConvertToModel(taskId); }
        }

        public IWebDriver Browser { get; set; }

        private
        TaskSection
        ConvertToModel(
            Guid taskId)
        {
            var taskElement = Browser.GetElementById(taskId);

            return new TaskSection(taskElement) {
                Id                 = taskId,
                Browser            = Browser
            };
        }
    }

    public class TaskSection
    {
        private readonly IWebElement _taskElement;

        public Guid         Id                  { get; set; }
        public IWebDriver   Browser             { get; set; }

        public 
        TaskSection(
            IWebElement taskElement)
        {
            _taskElement = taskElement;
        }

        public string Title
        {
            get
            {
                return _taskElement.GetElementByClass("Title").Text;
            }
        }

        public int CompletedVoteCount
        {
            get
            {
                return _taskElement.GetSubElementText<int>(
                    By.ClassName("CompletedVoteCount")
                );
            }
        }
    }
}