namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class CommandEnvelope
    {
        public string Name      { get; set; }
        public object Payload   { get; set; }

        public CommandEnvelope(object command)
        {
            Name    = command.GetType().Name;
            Payload = command;
        }
    }
}
