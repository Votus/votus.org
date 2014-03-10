using OpenQA.Selenium.Support.PageObjects;
using System;

namespace Votus.Testing.Integration.WebsiteModels
{
    class IdeaPageSection : BasePageSection
    {
        public Guid         Id      { get; set; }
        public string       Title   { get; set; }
        public string       Tag     { get; set; }
    
        private TaskListPageSection _taskListPage;

        public TaskListPageSection Tasks
        {
            get
            {
                // TODO: Use Ninject to inject this?
                if (_taskListPage != null) return _taskListPage;

                _taskListPage = new TaskListPageSection { Browser = Browser };

                PageFactory.InitElements(Browser, _taskListPage);

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
