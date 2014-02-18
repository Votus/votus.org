using System;
using System.Text.RegularExpressions;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class Idea
    {
        public Guid Id { get; set; }

        private string _title;

        public 
        string 
        Title
        {
            get { return _title; }
            set { _title = CleanString(value); }
        }

        private string _tag;

        public 
            string 
            Tag
        {
            get { return _tag; } 
            set { _tag = CleanString(value); }
        }

        #region ReSharper Generated Overrides
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((Idea) obj);
        }

        protected bool Equals(Idea other)
        {
            return string.Equals(Id,                 other.Id)
                && string.Equals(Title, other.Title)
                && string.Equals(Tag,   other.Tag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();

                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                
                return hashCode;
            }
        }

        #endregion

        private 
        static 
        string 
        CleanString(
            string input)
        {
            return Regex.Replace(
                (input ?? string.Empty).Trim(), 
                @"\s+", 
                " "
            );
        }

        public 
        override 
        string 
        ToString()
        {
            // TODO: Does this have to be JSON?

            return string.Format(
                "{{\"Id\":\"{0}\",\"Tag\":\"{1}\",\"Title\":\"{2}\"}}", 
                Id, 
                Tag, 
                Title
            );
        }
    }
}
