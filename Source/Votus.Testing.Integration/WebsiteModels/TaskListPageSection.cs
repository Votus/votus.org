using System;
using Votus.Testing.Integration.Acceptance;

namespace Votus.Testing.Integration.WebsiteModels
{
    class TaskListPageSection : BasePageSection
    {
        public TaskPageSection this[Guid taskId]
        {
            get { return ConvertToModel(taskId); }
        }
        
        private
        TaskPageSection
        ConvertToModel(
            Guid taskId)
        {
            var taskElement = Browser.GetElementById(taskId);

            return new TaskPageSection(taskElement)
            {
                Id = taskId,
                Browser = Browser
            };
        }
    }
}
