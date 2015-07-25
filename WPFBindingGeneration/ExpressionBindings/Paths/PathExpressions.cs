using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public static class PathExpressions
	{
		public static IEnumerable<IPathElement> GetPathElements(Expression expression)
		{
			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				var propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo == null)
				{
					return new[] { new ContextReference(Expression.Lambda(memberExpression).Compile().DynamicInvoke()) };
					//throw new ArgumentException("Access must be a property, and not a field.");	
				}
				return GetPathElements(memberExpression.Expression).Concat(new[] { new PropertyAccess(propertyInfo) });
			}

			var parameterExpression = expression as ParameterExpression;
			if (parameterExpression != null)
			{
				return new[] { new Parameter(parameterExpression) };
			}

			var value = expression as ConstantExpression;
			if (value != null)
			{
				return new[] { new ContextReference(value.Value) };
			}
			throw new ArgumentException("Expression given to PathExpressionBinding was not a path.");
		}
	}
}