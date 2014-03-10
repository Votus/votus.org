using System;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class CreateIdeaCommand
    {
        public const string ValidTitle   = "Valid Title";
        public const string VotusTestTag = "votus-test";

        public CreateIdeaCommand(
            string newIdeaTag   = VotusTestTag,
            string newIdeaTitle = ValidTitle)
        {
            NewIdeaId    = Guid.NewGuid();
            NewIdeaTag   = newIdeaTag;
            NewIdeaTitle = newIdeaTitle;
        }

        public Guid   NewIdeaId     { get; set; }
        public string NewIdeaTitle  { get; set; }
        public string NewIdeaTag    { get; set; }
    }
}
