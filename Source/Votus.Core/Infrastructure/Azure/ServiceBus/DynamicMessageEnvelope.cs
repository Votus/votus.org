using System;

namespace Votus.Core.Infrastructure.Azure.ServiceBus
{
    public class DynamicMessageEnvelope
    {
        public Guid   Id            { get; set; }
        public string PayloadType   { get; set; }
        public string Payload       { get; set; }
    }
}