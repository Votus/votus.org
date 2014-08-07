using System.Collections.Generic;
using System.Linq;

namespace Votus.Core.Infrastructure.Collections
{
    // TODO: Factor out this class

    public class ConsistentHashSet<T> : HashSet<T>
    {
        public ConsistentHashSet()                                             { } 
        public ConsistentHashSet(IEnumerable<T> collection) : base(collection) { }

        public override int GetHashCode()
        {
            // Aggregate the hash codes of all the items 
            // within to calculate the hash code for the whole set

            return this.Aggregate(
                seed: 0,
                func: (current, item) => (current*397) ^ item.GetHashCode()
            );
        }
    }
}
