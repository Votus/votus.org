using System.Collections.Generic;

namespace Votus.Core.Infrastructure.Collections
{
    public static class Extensions
    {
        public
        static
        bool
        Contains<T>(
            this
            IEnumerable<T> masterCollection,
            IEnumerable<T> subCollection
            )
        {
            return new HashSet<T>(masterCollection)
                .IsSupersetOf(subCollection);
        }
    }
}
