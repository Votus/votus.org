using OpenQA.Selenium;
using System;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class IdeaPageSection : BasePageSection
    {
        #region Properties

        public Guid     Id      { get { return PageSectionElement.GetAttributeValue<Guid>("Id"); } }
        public string   Tag     { get { return PageSectionElement.GetSubElementTextAs<string>(By.ClassName("Tag")); } }
        public string   Title   { get { return PageSectionElement.GetSubElementTextAs<string>(By.ClassName("Title")); } }

        public TaskListPageSection Tasks { get; set; }
        public GoalListPageSection Goals { get; set; }

        #endregion

        #region Constructors

        public 
        IdeaPageSection(
            IWebElement ideaElement) 
            : base(ideaElement)
        {
            Tasks = new TaskListPageSection(PageSectionElement.GetElementByClass("Tasks"));
            Goals = new GoalListPageSection(PageSectionElement.GetElementByClass("GoalsDisplay"));
        }

        #endregion

        #region Page Methods

        public
        void
        ShowIdeaDetails()
        {
            if (!PageSectionElement.GetElementByClass("IdeaBody").Displayed)
                PageSectionElement
                    .GetElementByClass("IdeaHeader")
                    .Click();
        }

        public
        GoalListPageSection
        ShowGoalsDisplay()
        {
            ShowIdeaDetails();

            var goalsDisplayElement = PageSectionElement.GetElementByClass("GoalsDisplay");

            if (!goalsDisplayElement.Displayed)
                PageSectionElement
                    .GetElementByClass("GoalsHeader")
                    .Click();

            return new GoalListPageSection(goalsDisplayElement);
        }

        public
        TaskListPageSection
        ShowTasksDisplay()
        {
            ShowIdeaDetails();

            var tasksDisplayElement = PageSectionElement.GetElementByClass("TasksDisplay");

            if (!tasksDisplayElement.Displayed)
                PageSectionElement
                    .GetElementByClass("TasksHeader")
                    .Click();

            return new TaskListPageSection(tasksDisplayElement);
        }

        #endregion

        #region ReSharper Generated Methods

        protected bool Equals(IdeaPageSection other)
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
            return Equals((IdeaPageSection) obj);
        }

        #endregion
    }
}
