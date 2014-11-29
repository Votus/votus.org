using System.Threading.Tasks;
using Ninject;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Domain.TestEntities
{
    public class TestEntitiesManager // TODO: Rename to just TestEntities (and all others *Managers too)
    {
        // TODO: Replace with a TestEntityRepository.
        [Inject] public IVersioningRepository<TestEntity> Repository { get; set; }

        public 
        Task 
        HandleAsync(
            CreateTestEntityCommand createTestEntityCommand)
        {
            var testEntity = new TestEntity(
                createTestEntityCommand.TestEntityId, 
                createTestEntityCommand.TestProperty
);

            return Repository.SaveAsync(testEntity);
        }
    }
}
