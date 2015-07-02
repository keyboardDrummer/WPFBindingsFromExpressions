using System;
using System.Linq;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;
using WPFBindingGeneration.ExpressionBindings.Paths;
using WPFBindingGeneration.Utility;

namespace WPFBindingGeneration
{
	public static class BindingGenerator
	{
		public static IExpressionBinding<From, To> TwoWay<From, To>(Expression<Func<From, To>> func)
		{
			var oneWay = OneWay(func);
			if (!oneWay.IsWritable)
				throw new ArgumentException();
			return oneWay;
		}

		public static IExpressionBinding<From, To> OneWay<From, To>(Expression<Func<From, To>> func)
		{
			var paths = new SortedSet<Expression>();
			var guid = Guid.NewGuid();
			var parameter = Expression.Parameter(typeof (object[]), guid.ToString());
			var newBody = ExtractPaths(parameter, func.Body, paths);
			var pathFuncs = paths.Select(path => Expression.Lambda(path, func.Parameters[0])).ToList();
			var parameterExpression = parameter;
			var converter = Expression.Lambda<Func<object[], To>>(newBody, parameterExpression).Compile();
			if (paths.Count == 1)
			{
				var path = paths[0];
				if (IsEndPoint(newBody, parameter))
				{
					return new PathExpressionBinding<From, To>(Expression.Lambda<Func<From, To>>(path, Expression.Parameter(typeof (From))));
				}
				var pathBinding = new PathExpressionBinding<From, object>(pathFuncs[0]);
				return pathBinding.Convert(value => new[] {value}).Convert(converter);
			}
			return new MultiPathExpressionBinding<From, To>(pathFuncs, converter);
		}

		static bool IsEndPoint(Expression body, Expression parameter)
		{
			if (body.NodeType != ExpressionType.Convert)
				return false;

			var convert = (UnaryExpression) body;
			if (convert.Operand.NodeType != ExpressionType.ArrayIndex)
				return false;
			var index = (BinaryExpression) convert.Operand;
			return index.Left == parameter;
		}

		static Expression ExtractPaths(ParameterExpression parameter, Expression expression, SortedSet<Expression> paths)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null)
			{
				var newLeft = ExtractPaths(parameter, binaryExpression.Left, paths);
				var newRight = ExtractPaths(parameter, binaryExpression.Right, paths);
				return Expression.MakeBinary(binaryExpression.NodeType, newLeft, newRight,
					binaryExpression.IsLiftedToNull, binaryExpression.Method);
			}
			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				var newOperand = ExtractPaths(parameter, unaryExpression.Operand, paths);
				return Expression.MakeUnary(unaryExpression.NodeType, newOperand, unaryExpression.Type, unaryExpression.Method);
			}
			var methodCall = expression as MethodCallExpression;
			if (methodCall != null)
			{
				var newArguments = methodCall.Arguments.Select(argument => ExtractPaths(parameter, argument, paths));
				return Expression.Call(methodCall.Object, methodCall.Method, newArguments);
			}
			var parameterExpression = expression as ParameterExpression;
			if (parameterExpression != null)
			{
				paths.Add(parameterExpression);
				return GetParameter(parameter, paths.Count - 1, parameter.Type);
			}
			var member = expression as MemberExpression;
			if (member != null)
			{
				paths.Add(member);
				return GetParameter(parameter, paths.Count - 1, member.Type);
			}
			var constant = expression as ConstantExpression;
			if (constant != null)
				return constant;

			throw new NotImplementedException();
		}

		static Expression GetParameter(ParameterExpression parameter, int index, Type type)
		{
			var arrayIndex = Expression.ArrayIndex(parameter, Expression.Constant(index));
			return Expression.Convert(arrayIndex, type);
		}

		public static PathExpressionBinding<From, To> Path<From, To>(Expression<Func<From, To>> func)
		{
			return new PathExpressionBinding<From, To>(func);
		}

		public static IExpressionBinding<From, To> Convert<From, To>(Func<From, To> func)
		{
			return Root<From>().Convert(func);
		}

		public static PathExpressionBinding<From, From> Root<From>()
		{
			return Path((From x) => x);
		}
	}
}