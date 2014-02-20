using System;

namespace Votus.Web.Areas.Api.Models
{
    public class TaskViewModel
    {
        public Guid     Id      { get; set; }
        public string   Title   { get; set; }

        #region ReSharper Generated Overrides

        protected bool Equals(TaskViewModel other)
        {
            return Id.Equals(other.Id) && string.Equals(Title, other.Title);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ Title.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        #endregion
    }
}