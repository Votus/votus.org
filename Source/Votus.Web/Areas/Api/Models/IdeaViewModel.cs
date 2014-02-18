using System;

namespace Votus.Web.Areas.Api.Models
{
    public class IdeaViewModel
    {
        public Guid     Id      { get; set; }
        public string   Title   { get; set; }
        public string   Tag     { get; set; }

        #region ReSharper Generated Overrides
        
        protected bool Equals(IdeaViewModel other)
        {
            return Id.Equals(other.Id) && string.Equals(Title, other.Title) && string.Equals(Tag, other.Tag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdeaViewModel) obj);
        }

        #endregion
    }
}