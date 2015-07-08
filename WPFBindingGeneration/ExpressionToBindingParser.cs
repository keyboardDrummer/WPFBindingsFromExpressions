using System;
using System.Linq;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;
using WPFBindingGeneration.ExpressionBindings.Paths;
using WPFBindingGeneration.Utility;

namespace WPFBindingGeneration
{
	// ReSharper disable once ConvertToStaticClass
	public sealed class Unit
	{
		Unit()
		{
		}
	}

	public static class ExpressionToBindingParser
	{
		public static IExpressionBinding<Unit, To> TwoWay<To>(Expression<Func<To>> func)
		{
			var oneWay = OneWay(func);
			if (!oneWay.IsWritable)
				throw new ArgumentException();
			return oneWay;
		}

		public static IExpressionBinding<From, To> TwoWay<From, To>(Expression<Func<From, To>> func)
		{
			var oneWay = OneWay(func);
			if (!oneWay.IsWritable)
				throw new ArgumentException();
			return oneWay;
		}

		public static IExpressionBinding<Unit, To> OneWay<To>(Expression<Func<To>> func)
		{
			return OneWay(Expression.Lambda<Func<Unit, To>>(func.Body, Expression.Parameter(typeof (Unit))));
		}

		public static IExpressionBinding<From, To> OneWay<From, To>(Expression<Func<From, To>> func)
		{
			var paths = new SortedSet<Expression>();
			var guid = Guid.NewGuid();
			var parameter = Expression.Parameter(typeof (object[]), guid.ToString());
			var newBody = ExtractPaths(parameter, func.Body, paths);
			var pathFuncs = paths.Select(path => Expression.Lambda(path, func.Parameters[0])).ToList();
			var converter = Expression.Lambda<Func<object[], To>>(newBody, parameter).Compile();
			if (pathFuncs.Count == 1)
			{
				if (IsEndPoint(newBody, parameter))
				{
					return new PathExpressionBinding<From, To>(pathFuncs[0]);
				}
				var pathBinding = new PathExpressionBinding<From, object>(pathFuncs[0]);
				return pathBinding.Convert(value => new[] {value}).Convert(converter);
			}
			return new MultiPathExpressionBinding<From, To>(pathFuncs, converter, null);
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
				return GetParameter(parameter, paths.Count - 1, parameterExpression.Type);
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
	}
}