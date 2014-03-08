using System;

namespace Votus.Web.Areas.Api.Models
{
    public class TaskViewModel
    {
        public Guid     Id                  { get; set; }
        public string   Title               { get; set; }
        public int      CompletedVoteCount  { get; set; }

        #region ReSharper Generated Overrides

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskViewModel) obj);
        }

        protected bool Equals(TaskViewModel other)
        {
            return Id.Equals(other.Id) && string.Equals(Title, other.Title) && CompletedVoteCount == other.CompletedVoteCount;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Title.GetHashCode();
                hashCode = (hashCode * 397) ^ CompletedVoteCount;
                return hashCode;
            }
        }

        #endregion
    }
}