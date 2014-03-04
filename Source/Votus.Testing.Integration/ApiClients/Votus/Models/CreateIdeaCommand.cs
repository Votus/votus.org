using System;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class CreateIdeaCommand
    {
        public const string ValidTitle   = "Valid Title";
        public const string VotusTestTag = "votus-test";

        public CreateIdeaCommand(
            string tag          = VotusTestTag,
            string newIdeaTitle = ValidTitle)
        {
            NewIdeaId    = Guid.NewGuid();
            Tag          = tag;
            NewIdeaTitle = newIdeaTitle;
        }

        public Guid   NewIdeaId     { get; set; }
        public string NewIdeaTitle  { get; set; }
        public string Tag           { get; set; }
    }
}
