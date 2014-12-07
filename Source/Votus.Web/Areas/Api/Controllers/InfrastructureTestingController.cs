using System;
using System.Collections.Generic;
using Ninject;
using System.Threading.Tasks;
using System.Web.Http;
using Votus.Core.Domain;
using Votus.Core.Infrastructure.Data;
using Votus.Core.Infrastructure.Queuing;
using Task = System.Threading.Tasks.Task;

namespace Votus.Web.Areas.Api.Controllers
{
    /// <summary>
    /// This controller provides APIs for exercising various parts of the infrastructure for test purposes.
    /// </summary>
    [RoutePrefix(ApiAreaRegistration.AreaRegistrationName + "/infrastructure-testing")]
    public class InfrastructureTestingController : ApiController
    {
        [Inject] public QueueManager            CommandDispatcher   { get; set; }
        [Inject] public TestEntityRepository TestEntityViewModelRepository          { get; set; }

        [HttpPost]
        [Route("test-entities")]
        public 
        Task 
        CreateTestEntity(
            CreateTestEntityCommand createTestEntityCommand)
        {
            return CommandDispatcher.SendAsync(
                commandId: createTestEntityCommand.Id,
                command:   createTestEntityCommand
            );
        }

        [HttpGet]
        [Route("test-entities/recent")]
        public 
        Task<IEnumerable<TestEntityViewModel>> 
        GetRecentTestEntities()
        {
            return TestEntityViewModelRepository.GetRecent();
        }
    }

    public class TestEntityViewModel
    {
        public Guid     Id              { get; set; }
        public string   TestProperty    { get; set; }
    }

    public class TestEntityRepository
    {
        [Inject] public IKeyValueRepository Repository { get; set; }

        public Task<IEnumerable<TestEntityViewModel>> GetRecent()
        {
            return Repository.GetAsync<IEnumerable<TestEntityViewModel>>(
                "recent-test-entities"
            );
        }
    }
}