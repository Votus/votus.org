using System.Diagnostics;
using System.Linq;
using Votus.Testing.Integration.ApiClients.Votus;
using Votus.Testing.Integration.ApiClients.Votus.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Votus.Testing.Integration.Performance
{
    public class ApiPerformanceTests
    {
        readonly VotusApiClient _apiClient = SharedResources.Get<VotusApiClient>();

        [Fact]
        public
        async Task 
        CommandsApi_SubmitConcurrentCommands_AverageResponseTimeBelowMaximum()
        {
            // Arrange
            const int TotalCommands          = 10;
            const int MaxAverageMilliseconds = 150;
            
            var createCommands = Enumerable
                .Range(0, TotalCommands)
                .Select(i => new CreateIdeaCommand(newIdeaTitle: "PerfTest " + i));

            var stopwatch = Stopwatch.StartNew();

            // Act
            var createTasks = createCommands.Select(command => 
                _apiClient.Commands.SendAsync(command)
            );

            await Task.WhenAll(createTasks);
            
            // Assert
            Assert.InRange(
                actual: (stopwatch.ElapsedMilliseconds / TotalCommands), 
                low:    0, 
                high:   MaxAverageMilliseconds
            );
        }
    }
}