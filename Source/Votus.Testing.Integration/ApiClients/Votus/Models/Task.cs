namespace Votus.Testing.Integration.ApiClients.Votus.Models
{
    class Task
    {
        public string Id    { get; set; }
        public string Title { get; set; }

        #region ReSharper Generated Methods

        public override bool Equals(object obj)
        {
            return Equals((Task)obj);
        }

        protected bool Equals(Task other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Title, other.Title);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ Title.GetHashCode();
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Id: {0}, Title: {1}", Id, Title);
        }
    }
}
