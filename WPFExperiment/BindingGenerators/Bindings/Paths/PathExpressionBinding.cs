using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Data;
using WPFExperiment.BindingGenerators.Bindings;

namespace WPFExperiment.BindingGenerators
{
	public class PathExpressionBinding<From, To> : DefaultExpressionBinding
	{
		readonly LambdaExpression func;

		public PathExpressionBinding(LambdaExpression func)
		{
			this.func = func;
		}

		public override bool IsWritable
		{
			get { return GetElements().Last().Writable; }
		}

		static IEnumerable<IPathElement> GetPathElements(Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				var propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo == null)
					throw new ArgumentException("Access must be a property, and not a field.");
				return GetPathElements(memberExpression.Expression).Concat(new[] {new PropertyAccess(propertyInfo)});
			}

			var parameterExpression = expression as ParameterExpression;
			if (parameterExpression != null)
			{
				return new[] {new Parameter(parameterExpression)};
			}
			throw new ArgumentException("Expression given to PathExpressionBinding was not a path.");
		}

		public Binding ToBinding()
		{
			var result = new Binding(string.Join(".", GetElements().Select(element => element.ToString()).Where(s => !string.IsNullOrEmpty(s))));
			result.Mode = IsWritable ? BindingMode.TwoWay : BindingMode.OneWay;
			result.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			return result;
		}

		public override BindingBase ToBindingBase()
		{
			return ToBinding();
		}

		IEnumerable<IPathElement> GetElements()
		{
			return GetPathElements(func.Body);
		}

		public ConvertedPathExpressionBinding<From, To, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null)
		{
			return new ConvertedPathExpressionBinding<From, To, NewTo>(this, forward, backward);
		}
	}
}