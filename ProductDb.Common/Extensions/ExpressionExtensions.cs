using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProductDb.Common.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<T> Compose<T>(this Expression<T> First, Expression<T> Second, Func<Expression, Expression, Expression> Merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = First.Parameters.Select((f, i) => new { f, s = Second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, Second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(Merge(First.Body, secondBody), First.Parameters);
        }

        /// <summary>
        /// Extension method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> First, Expression<Func<T, bool>> Second)
        {
            return First.Compose(Second, Expression.And);
        }

        /// <summary>
        /// Extension method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="First"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> First, Expression<Func<T, bool>> second)
        {
            return First.Compose(second, Expression.Or);
        }
    }
}
