using System;
using System.Linq;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;
using WPFBindingGeneration.ExpressionBindings.Paths;

namespace WPFBindingGeneration
{
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
			var result = ExtractPaths(func.Body);
			var paths = result.Paths;
			var parameter = func.Parameters[0];
			var pathFuncs = paths.Select(path => Expression.Lambda(path, parameter)).ToList();

			if (pathFuncs.Count == 1)
			{
				var objectParameter = Expression.Parameter(typeof (object));
				var newBody = result.CreateExpression((path, type) => Expression.Convert(objectParameter, type));
				if (IsEndPoint(newBody, objectParameter))
				{
					return new PathExpressionBinding<From, To>(pathFuncs[0]);
				}
				var pathBinding = new PathExpressionBinding<From, object>(pathFuncs[0]);
				var converter = Expression.Lambda<Func<object, To>>(newBody, objectParameter).DebugCompile();
				return pathBinding.Convert(converter);
			}
			else
			{
				var arrayParameter = Expression.Parameter(typeof (object[]));
				var pathIndices = paths.Select((e, i) => Tuple.Create(e, i)).ToDictionary(t => t.Item1, t => t.Item2);

				var arrayParameterBody = result.CreateExpression((path, type) => GetArrayParameter(arrayParameter, pathIndices[path], type));

				var converter = Expression.Lambda<Func<object[], To>>(arrayParameterBody, arrayParameter).DebugCompile();
				return new MultiPathExpressionBinding<From, To>(pathFuncs, converter, null);
			}
		}

		static bool IsEndPoint(Expression body, Expression parameter)
		{
			if (body.NodeType != ExpressionType.Convert)
				return false;

			var index = (UnaryExpression) body;
			return index.Operand == parameter;
		}

		/// <summary>
		/// Returns an updated expression with all the paths replaced by parameters.
		/// </summary>
		static ExtractPathsResult<Expression> ExtractPaths(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null)
			{
				return ParseBinary(binaryExpression);
			}
			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				var operandResult = ExtractPaths(unaryExpression.Operand);
				return operandResult.Select(operand => (Expression) Expression.MakeUnary(unaryExpression.NodeType, operand, unaryExpression.Type, unaryExpression.Method));
			}
			var methodCall = expression as MethodCallExpression;
			if (methodCall != null)
			{
				return ParseMethodCall(methodCall);
			}
			var parameterExpression = expression as ParameterExpression;
			if (parameterExpression != null)
			{
				return new ExtractPathsResult<Expression>(createParameter => createParameter(parameterExpression, parameterExpression.Type), parameterExpression);
			}
			var member = expression as MemberExpression;
			if (member != null)
			{
				return ParseMemberExpression(member);
			}
			var constant = expression as ConstantExpression;
			if (constant != null)
				return new ExtractPathsResult<Expression>(p => constant);

			var conditional = expression as ConditionalExpression;
			if (conditional != null)
			{
				var conditionResult = ExtractPaths(conditional.Test);
				var thenResult = ExtractPaths(conditional.IfTrue);
				var elseResult = ExtractPaths(conditional.IfFalse);
				return thenResult.Combine(elseResult, Tuple.Create).Combine(conditionResult,
					(thenAndElse, condition) => (Expression) Expression.Condition(condition, thenAndElse.Item1, thenAndElse.Item2, conditional.Type));
			}
			throw new NotImplementedException();
		}

		static ExtractPathsResult<Expression> ParseBinary(BinaryExpression binaryExpression)
		{
			var leftResult = ExtractPaths(binaryExpression.Left);
			var rightResult = ExtractPaths(binaryExpression.Right);
			return leftResult.Combine(rightResult, (left, right) => (Expression) Expression.MakeBinary(binaryExpression.NodeType, left, right,
				binaryExpression.IsLiftedToNull, binaryExpression.Method));
		}

		static ExtractPathsResult<Expression> ParseMethodCall(MethodCallExpression methodCall)
		{
			var argumentResults = methodCall.Arguments.Select(ExtractPaths);
			var argumentsResult = ExtractPathsResult<Expression>.Flatten(argumentResults);
			var objectResult = methodCall.Object == null
				? new ExtractPathsResult<Expression>(f => null)
				: ExtractPaths(methodCall.Object);
			return objectResult.Combine(argumentsResult, (newObject, newArguments) => (Expression) Expression.Call(newObject, methodCall.Method, newArguments));
		}

		static ExtractPathsResult<Expression> ParseMemberExpression(MemberExpression member)
		{
			var pathExpression = PathExpressions.GetPathElements(member);
			if (pathExpression == null)
				return ExtractPaths(member.Expression).Select(newMember => (Expression) Expression.PropertyOrField(newMember, member.Member.Name));
			else
				return new ExtractPathsResult<Expression>(createParameter => createParameter(member, member.Type), member);
		}

		static Expression GetArrayParameter(ParameterExpression parameter, int index, Type type)
		{
			var arrayIndex = Expression.ArrayIndex(parameter, Expression.Constant(index));
			return Expression.Convert(arrayIndex, type);
		}
	}
}