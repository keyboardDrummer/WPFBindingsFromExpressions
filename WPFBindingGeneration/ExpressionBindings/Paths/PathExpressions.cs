using System.Linq.Expressions;
using System.Reflection;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public static class PathExpressions
	{
		public static IPathElement ParsePath(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Convert)
			{
				var conversion = (UnaryExpression)expression;
				return ParsePath(conversion.Operand);
			}

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				var propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo == null)
				{
					return new ContextReference(Expression.Lambda(memberExpression).Compile().DynamicInvoke());
					//throw new ArgumentException("Access must be a property, and not a field.");	
				}
				if (propertyInfo.GetMethod.IsStatic)
				{
					return new ContextReference(propertyInfo.GetValue(null)); //Sure I want to evaluate now?
				}

				var recursive = ParsePath(memberExpression.Expression);
				return recursive == null ? null : new PropertyAccess(propertyInfo, recursive);
			}

			var parameterExpression = expression as ParameterExpression;
			if (parameterExpression != null)
			{
				return new Parameter(parameterExpression);
			}

			var value = expression as ConstantExpression;
			if (value != null)
			{
				return new ContextReference(value.Value);
			}
			return null;
		}
	}
}
