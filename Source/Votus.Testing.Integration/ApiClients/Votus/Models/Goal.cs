using System;

namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class Goal
    {
        public Guid   Id    { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Id: {0}, Title: {1}", 
                Id, 
                Title
            );
        }

        #region ReSharper Generated Methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Goal) obj);
        }

        protected bool Equals(Goal other)
        {
            return Id.Equals(other.Id) && string.Equals(Title, other.Title);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ Title.GetHashCode();
            }
        }

        #endregion
    }
}
