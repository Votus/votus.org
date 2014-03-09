using System;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    public class CreateTaskCommand
    {
        public const string ValidTitle   = "Valid Title";

        public CreateTaskCommand(
            Guid    ideaId,
            string  newTaskTitle = ValidTitle)
        {
            IdeaId       = ideaId;
            NewTaskId    = Guid.NewGuid();
            NewTaskTitle = newTaskTitle;
        }

        public Guid   IdeaId        { get; set; }
        public Guid   NewTaskId     { get; set; }
        public string NewTaskTitle  { get; set; }
    }
}
