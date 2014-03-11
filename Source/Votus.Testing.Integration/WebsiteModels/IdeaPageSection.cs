using OpenQA.Selenium;
using System;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class IdeaPageSection : BasePageSection
    {
        public Guid         Id      { get; set; }
        public string       Title   { get; set; }
        public string       Tag     { get; set; }
    
        private TaskListPageSection _taskListPage;
        private IWebElement         _ideaElement;

        public 
        IdeaPageSection(
            IWebElement ideaElement) : base(null)
        {
            _ideaElement = ideaElement;

            Id    = _ideaElement.GetAttributeValue<Guid>("Id");
            Tag   = _ideaElement.GetSubElementText<string>(By.ClassName("Tag"));
            Title = _ideaElement.GetSubElementText<string>(By.ClassName("Title"));
        }

        public TaskListPageSection Tasks
        {
            get
            {
                // TODO: Use Ninject to inject this?
                if (_taskListPage != null) return _taskListPage;

                _taskListPage = new TaskListPageSection(
                    _ideaElement.GetElementByClass("Tasks")
                );

                return _taskListPage;
            }
        }

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
