using System;
using System.ComponentModel.DataAnnotations;

namespace Votus.Core.Tasks
{
    public class VoteTaskCompletedCommand
    {
        [Required]
        public Guid TaskId { get; set; }
    }
}
