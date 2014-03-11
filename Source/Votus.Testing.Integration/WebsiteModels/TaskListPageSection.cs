using System;
using OpenQA.Selenium;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class TaskListPageSection : BasePageSection
    {
        public TaskPageSection this[Guid taskId]
        {
            get { return ConvertToModel(taskId); }
        }

        public
        TaskListPageSection(
            IWebElement taskListElement)
            : base(taskListElement)
        {
        }

        private
        TaskPageSection
        ConvertToModel(
            Guid taskId)
        {
            return new TaskPageSection(
                PageSectionElement.GetElementById(taskId)
            );
        }
    }
}
