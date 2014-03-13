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

        public 
        TaskPageSection
        SubmitTask(
            string taskTitle)
        {
            // Get the id that will be used for the new task
            var taskId = PageSectionElement
                .GetElementByClass("NewTaskId")
                .GetAttributeValue<Guid>("value");

            // Input the task title
            SendKeysToNewTaskTitle(taskTitle);

            var newTaskButton = PageSectionElement.GetElementByClass("NewTaskButton");

            if (!newTaskButton.Displayed)
                throw new Exception(GetCurrentErrorMessage());

            PageSectionElement
                .GetElementByClass("NewTaskButton")
                .Click();

            return this[taskId];
        }

        public
        string
        GetCurrentErrorMessage()
        {
            return PageSectionElement
                .GetSubElementTextAs<string>(By.ClassName("field-validation-error"));
        }

        public 
        void
        SendKeysToNewTaskTitle(
            string newTaskTitle)
        {
            PageSectionElement
                .GetElementByClass("NewTaskTitle")
                .SendKeys(newTaskTitle);
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
