using System;

namespace Votus.Core.Domain.TestEntities
{
    public class CreateTestEntityCommand
    {
        public Guid     TestEntityId { get; set; }
        public string   TestProperty { get; set; }
    }
}
