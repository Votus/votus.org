using System;
using OpenQA.Selenium;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class TaskListPageSection : BasePageSection
    {
        private IWebElement _taskListElement;

        public TaskPageSection this[Guid taskId]
        {
            get { return ConvertToModel(taskId); }
        }

        public 
        TaskListPageSection(
            IWebElement taskListElement)
            : base(null)
        {
            _taskListElement = taskListElement;
        }

        private
        TaskPageSection
        ConvertToModel(
            Guid taskId)
        {
            var taskElement = _taskListElement.GetElementById(taskId);

            return new TaskPageSection(taskElement);
        }
    }
}
