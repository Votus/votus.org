using System;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    public class VoteTaskCompletedCommand
    {
        Guid TaskId { get; set; }

        public VoteTaskCompletedCommand(Guid taskId)
        {
            TaskId = taskId;
        }
    }
}
