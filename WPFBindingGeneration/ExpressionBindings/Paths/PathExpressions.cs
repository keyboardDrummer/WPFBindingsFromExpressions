using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public static class PathExpressions
	{
		public static readonly MethodInfo CurrentMethod = typeof(CurrentExtension).GetMethod(nameof(CurrentExtension.Current));

		public static IPathExpression ParsePath(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}

			if (expression.NodeType == ExpressionType.Convert)
			{
				var conversion = (UnaryExpression)expression;
				return ParsePath(conversion.Operand);
			}

			var callExpression = expression as MethodCallExpression;
			if (callExpression != null)
			{
				return ParseMethodCall(callExpression);
			}

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
			{
				return ParseMember(memberExpression);
			}

			var parameterExpression = expression as ParameterExpression;
			if (parameterExpression != null)
			{
				return new Parameter(parameterExpression);
			}

			var value = expression as ConstantExpression;
			if (value != null && value.Value != null && value.Type.IsClass)
			{
				return new ContextReference(value.Value, value.Type);
			}
			return null;
		}

		private static IPathExpression ParseMember(MemberExpression memberExpression)
		{
			var propertyInfo = memberExpression.Member as PropertyInfo;
			if (propertyInfo == null)
			{
				return new ContextReference(Expression.Lambda(memberExpression).Compile().DynamicInvoke(), memberExpression.Type);
				//throw new ArgumentException("Access must be a property, and not a field.");	
			}
			if (propertyInfo.GetMethod.IsStatic)
			{
				return new ContextReference(propertyInfo.GetValue(null), memberExpression.Type); //Sure I want to evaluate now?
			}

			var recursive = ParsePath(memberExpression.Expression);
			return recursive == null ? null : new PropertyAccess(propertyInfo, recursive);
		}

		private static IPathExpression ParseMethodCall(MethodCallExpression callExpression)
		{
			var callExpressionMethod = callExpression.Method;
			var methodInfo = callExpressionMethod.IsGenericMethod ? callExpressionMethod.GetGenericMethodDefinition() : callExpressionMethod;
			if (methodInfo == CurrentMethod)
			{
				var inner = ParsePath(callExpression.Arguments[0]);
				return inner == null ? null : new CurrentPath(inner);
			}
			else if (callExpressionMethod.Name == "get_Item")
			{
				return ParseIndexerCall(callExpression, callExpressionMethod);
			}
			else
			{
				return null;
			}
		}

		private static IPathExpression ParseIndexerCall(MethodCallExpression callExpression, MethodInfo callExpressionMethod)
		{
			var inner = ParsePath(callExpression.Object);
			if (inner == null)
			{
				return null;
			}
			var getter = callExpressionMethod;
			var setter = getter.DeclaringType.GetProperties().Single(p => p.GetGetMethod().Equals(getter)).GetSetMethod();
			var indices = callExpression.Arguments.Select(argument => Expression.Lambda(argument).Compile().DynamicInvoke()).ToArray();
			return new IndexPath(inner, getter, setter, indices);
		}
	}
}
