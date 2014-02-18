namespace Votus.Core.Infrastructure.Messaging
{
    public class CommandEnvelope
    {
        public string Name       { get; set; }
        public object Payload    { get; set; }
    }
}
