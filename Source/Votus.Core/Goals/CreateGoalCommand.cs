using System;
using System.ComponentModel.DataAnnotations;

namespace Votus.Core.Goals
{
    public class CreateGoalCommand
    {
        public Guid NewGoalId   { get; set; }
        public Guid IdeaId      { get; set; }

        [Required(                                  ErrorMessage = "Please say a few words about your goal")]
        [RegularExpression(@"^.*(\w+)\s+(\w+).*$",  ErrorMessage = "Please say a few words about your goal")]
        public string NewGoalTitle { get; set; }
    }
}
