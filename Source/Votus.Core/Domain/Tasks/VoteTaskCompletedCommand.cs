using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Votus.Core.Infrastructure.Data;

namespace Votus.Core.Domain.Tasks
{
    [OncePer("You have already voted that this task is completed.")]
    public class VoteTaskCompletedCommand
    {
        [Required] public Guid      TaskId   { get; set; } // TODO: Validate that TaskId exists
        [Required] public string    VoterId  { get; set; }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", TaskId, VoterId);
        }
    }

    // TODO: Find a better location for this class.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OncePerAttribute : ValidationAttribute
    {
        //[Inject] Couldn't get property injection to work.
        public IRepository<VoteTaskCompletedCommand> ValueHashCodeRepository { get; set; }

        public 
        OncePerAttribute(
            string errorMessage,
            IRepository<VoteTaskCompletedCommand> valueHashCodeRepository) : base(errorMessage)
        {
            ValueHashCodeRepository = valueHashCodeRepository;
        }

        public OncePerAttribute(
            string errorMessage) : this(
                errorMessage, 
                DependencyResolver.Current.GetService<IRepository<VoteTaskCompletedCommand>>()) // TODO: Remove once property injection is working
        {
        }

        public 
        override 
        bool 
        IsValid(
            object value)
        {
            return !ValueExists(value);
        }

        private 
        bool 
        ValueExists(
            object value)
        {
            var key = string.Format(
                "{0}:{1}", 
                value.GetType(), 
                value.GetHashCode()
            );

            // This call is intentionally synchronous; expecting to hit a fast local in memory cache most of the time.
            return ValueHashCodeRepository.Exists(key);
        }
    }
}