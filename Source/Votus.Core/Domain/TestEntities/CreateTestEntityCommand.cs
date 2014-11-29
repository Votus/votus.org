using System;
using System.ComponentModel.DataAnnotations;

namespace Votus.Core.Domain.TestEntities
{
    public class CreateTestEntityCommand
    {
        [Required]
        public Guid     Id              { get; set; }
        public string   TestProperty    { get; set; }
    }
}
