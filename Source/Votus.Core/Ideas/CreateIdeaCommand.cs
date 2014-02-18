using System;
using System.ComponentModel.DataAnnotations;

namespace Votus.Core.Ideas
{
    public class CreateIdeaCommand
    {
        public Guid     NewIdeaId   { get; set; }
        public string   Tag         { get; set; }

        [Required(                                  ErrorMessage = "It would be cool if you could say a few words about your idea!")]
        [RegularExpression(@"^.*(\w+)\s+(\w+).*$",  ErrorMessage = "It would be cool if you could say a few words about your idea!")]
        public string NewIdeaTitle { get; set; }
    }
}