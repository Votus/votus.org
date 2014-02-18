using System;
using System.ComponentModel.DataAnnotations;

namespace Votus.Core.Tasks
{
    public class CreateTaskCommand
    {
        public Guid     NewTaskId       { get; set; }
        public Guid     IdeaId          { get; set; }

        [Required(                                  ErrorMessage = "Please say a few words about your task")]
        [RegularExpression(@"^.*(\w+)\s+(\w+).*$",  ErrorMessage = "Please say a few words about your task")]
        public string   NewTaskTitle    { get; set; }
    }
}
