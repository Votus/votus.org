using System;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class Task
    {
        public Guid   Id                { get; set; }
        public string Title             { get; set; }
        public int CompletedVoteCount   { get; set; }

        public 
        override 
        string 
        ToString()
        {
            return string.Format(
                "Id: {0}, Title: {1}, CompletedVoteCount: {2}", 
                Id, 
                Title, 
                CompletedVoteCount
            );
        }

        protected 
        bool 
        Equals(
            Task other)
        {
            return ToString() == other.ToString();
        }

        public 
        override 
        int 
        GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public 
        override 
        bool 
        Equals(
            object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Task) obj);
        }
    }
}
