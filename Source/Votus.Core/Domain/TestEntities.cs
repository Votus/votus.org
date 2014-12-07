using System;
using System.ComponentModel.DataAnnotations;
using Ninject;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Domain
{
    public class TestEntities
    {
        // TODO: Replace with a TestEntityRepository.
        [Inject] public IVersioningRepository<TestEntity> Repository { get; set; }

        public 
        System.Threading.Tasks.Task 
        HandleAsync(
            CreateTestEntityCommand createTestEntityCommand)
        {
            var testEntity = new TestEntity(
                createTestEntityCommand.Id, 
                createTestEntityCommand.TestProperty
            );

            return Repository.SaveAsync(testEntity);
        }
    }

    public class CreateTestEntityCommand
    {
        [Required]
        public Guid     Id              { get; set; }
        public string   TestProperty    { get; set; }
    }
}
