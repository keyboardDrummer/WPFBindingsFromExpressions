using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
    class PathBinding<From, To> : DefaultBindingSpec<From, To>
    {
        private readonly Expression<Func<From, To>> func;

        public PathBinding(Expression<Func<From, To>> func)
        {
            this.func = func;
        }

        private static IEnumerable<IPathElement> GetPathElements(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo == null)
                    throw new ArgumentException("access must be a property");
                return GetPathElements(memberExpression.Expression).Concat(new[] { new PropertyAccess(propertyInfo) });
            }

            var parameterExpression = expression as ParameterExpression;
            if (parameterExpression != null)
            {
                return new[] { new Parameter(parameterExpression) };
            }
            throw new ArgumentException();
        }

        public override Binding ToBindingBase()
        {
            return new Binding(String.Join(".", GetElements().Select(element => element.ToString()).Where(s => !string.IsNullOrEmpty(s))));
        }

        private IEnumerable<IPathElement> GetElements()
        {
            return GetPathElements(func.Body);
        }

        public override bool IsWritable
        {
            get { return GetElements().Last().Writable; }
        }

        public ConverterBinding<From, To, NewTo> Convert<NewTo>(Func<To, NewTo> forward, Func<NewTo, To> backward = null)
        {
            return new ConverterBinding<From, To, NewTo>(this,forward,backward);
        }
    }
}