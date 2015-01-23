using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WPFExperiment.BindingGenerators
{
    internal static class BindingGenerator
    {
        public static IBindingSpec<T,U> Path<T, U>(Expression<Func<T, U>> func)
        {
            var elements = GetPathElements(func.Body);
            return new PathBinding<T, U>(elements.ToList());
        }

        public static IBindingSpec<T, U> Convert<T, U>(Func<T, U> func)
        {
            return Root<T>().Convert(func);
        } 

        public static IBindingSpec<T,T> Root<T>()
        {
            return Path((T x) => x);
        }

        private static IEnumerable<IPathElement> GetPathElements(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo == null)
                    throw new ArgumentException("access must be a property");
                return GetPathElements(memberExpression.Expression).Concat(new[] {new PropertyAccess(propertyInfo)});
            }

            var parameterExpression = expression as ParameterExpression;
            if (parameterExpression != null)
            {
                return new[] {new Parameter(parameterExpression)};
            }
            throw new ArgumentException();
        }
    }
}
