using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Votus.Core.Infrastructure.Reflection
{
    public class DelegateAdjuster
    {
        public
        static
        Func<BaseT, Task>
        CastArgument<BaseT, DerivedT>(Expression<Func<DerivedT, Task>> source)
            where DerivedT : BaseT
        {
            if (typeof(DerivedT) == typeof(BaseT))
                return (Func<BaseT, Task>)((Delegate)source.Compile());

            var sourceParameter = Expression.Parameter(typeof(BaseT), "source");

            var result = Expression.Lambda<Func<BaseT, Task>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(
                        sourceParameter,
                        typeof(DerivedT)
                    )
                ),
                sourceParameter
            );

            return result.Compile();
        }
    }
}
