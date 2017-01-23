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
			{
				throw new ArgumentException("Given func cannot be bound two ways.");
			}
			return oneWay;
		}

		public static IExpressionBinding<From, To> TwoWay<From, To>(Expression<Func<From, To>> func)
		{
			var oneWay = OneWay(func);
			if (!oneWay.IsWritable)
			{
				throw new ArgumentException();
			}
			return oneWay;
		}

		public static IExpressionBinding<Unit, To> OneWay<To>(Expression<Func<To>> func)
		{
			return OneWay(Expression.Lambda<Func<Unit, To>>(func.Body, Expression.Parameter(typeof(Unit))));
		}

		public static IExpressionBinding<From, To> OneWay<From, To>(Expression<Func<From, To>> func)
		{
			var result = ExtractPaths(func.Body);
			var paths = result.Paths;

			if (paths.Count == 1)
			{
				var objectParameter = Expression.Parameter(typeof(object));
				var newBody = result.CreateExpression((path, type) => Expression.Convert(objectParameter, type));
				if (IsEndPoint(newBody, objectParameter))
				{
					return new PathExpressionBinding<From, To>(paths[0]);
				}
				var pathBinding = new PathExpressionBinding<From, object>(paths[0]);
				var converter = Expression.Lambda<Func<object, To>>(newBody, objectParameter).DebugCompile(func);
				return pathBinding.Convert(converter);
			}
			else
			{
				var arrayParameter = Expression.Parameter(typeof(object[]));
				var pathIndices = paths.Select((e, i) => Tuple.Create(e, i)).
					ToDictionary(t => t.Item1, t => t.Item2, ExtractPathResult<Expression>.Comparer);

				var arrayParameterBody = result.CreateExpression((path, type) => GetArrayParameter(arrayParameter, pathIndices[path], type));

				var converter = Expression.Lambda<Func<object[], To>>(arrayParameterBody, arrayParameter).DebugCompile(func);
				return new MultiPathExpressionBinding<From, To>(paths, converter, null);
			}
		}

		private static bool IsEndPoint(Expression body, Expression parameter)
		{
			if (body.NodeType != ExpressionType.Convert)
			{
				return false;
			}

			var index = (UnaryExpression)body;
			return index.Operand == parameter;
		}

		/// <summary>
		/// Returns an updated expression with all the paths replaced by parameters.
		/// </summary>
		private static ExtractPathResult<Expression> ExtractPaths(Expression expression)
		{
			var constant = expression as ConstantExpression;
			if (constant != null)
			{
				return new ExtractPathResult<Expression>(p => constant);
			}

			var path = PathExpressions.ParsePath(expression);
			if (path != null)
			{
				return new ExtractPathResult<Expression>(c => c(path, expression.Type), path);
			}

			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null)
			{
				return ParseBinary(binaryExpression);
			}
			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
			{
				var operandResult = ExtractPaths(unaryExpression.Operand);
				return operandResult.Select(operand => (Expression)Expression.MakeUnary(unaryExpression.NodeType, operand, unaryExpression.Type, unaryExpression.Method));
			}
			var methodCall = expression as MethodCallExpression;
			if (methodCall != null)
			{
				return ParseMethodCall(methodCall);
			}
			var member = expression as MemberExpression;
			if (member != null)
			{
				return ExtractPaths(member.Expression).Select(newMember => (Expression)Expression.PropertyOrField(newMember, member.Member.Name));
			}

			var conditional = expression as ConditionalExpression;
			if (conditional != null)
			{
				return ParseConditional(conditional);
			}
			var invoke = expression as InvocationExpression;
			if (invoke != null)
			{
				return ParseInvocation(invoke);
			}

			var binaryType = expression as TypeBinaryExpression;
			if (binaryType != null)
			{
				return ParseBinaryType(binaryType);
			}
			throw new NotImplementedException();
		}

		private static ExtractPathResult<Expression> ParseConditional(ConditionalExpression conditional)
		{
			var conditionResult = ExtractPaths(conditional.Test);
			var thenResult = ExtractPaths(conditional.IfTrue);
			var elseResult = ExtractPaths(conditional.IfFalse);
			return thenResult.Combine(elseResult, Tuple.Create).Combine(conditionResult,
				(thenAndElse, condition) => (Expression)Expression.Condition(condition, thenAndElse.Item1, thenAndElse.Item2, conditional.Type));
		}

		private static ExtractPathResult<Expression> ParseInvocation(InvocationExpression invoke)
		{
			var argumentResults = invoke.Arguments.Select(ExtractPaths);
			var argumentsResult = ExtractPathResult<Expression>.Flatten(argumentResults);
			var expressionResult = ExtractPaths(invoke.Expression);
			return expressionResult.Combine(argumentsResult, (newObject, newArguments) => (Expression)Expression.Invoke(newObject, newArguments));
		}

		private static ExtractPathResult<Expression> ParseBinaryType(TypeBinaryExpression binaryType)
		{
			return ExtractPaths(binaryType.Expression).Select<Expression>(newExpression =>
			{
				switch (binaryType.NodeType)
				{
					case ExpressionType.TypeAs:
						return Expression.TypeAs(newExpression, binaryType.TypeOperand);
					case ExpressionType.TypeEqual:
						return Expression.TypeEqual(newExpression, binaryType.TypeOperand);
					case ExpressionType.TypeIs:
						return Expression.TypeIs(newExpression, binaryType.TypeOperand);
					default:
						throw new NotSupportedException();
				}
			});
		}

		private static ExtractPathResult<Expression> ParseBinary(BinaryExpression binaryExpression)
		{
			var leftResult = ExtractPaths(binaryExpression.Left);
			var rightResult = ExtractPaths(binaryExpression.Right);
			return leftResult.Combine(rightResult, (left, right) => (Expression)Expression.MakeBinary(binaryExpression.NodeType, left, right,
				binaryExpression.IsLiftedToNull, binaryExpression.Method));
		}

		private static ExtractPathResult<Expression> ParseMethodCall(MethodCallExpression methodCall)
		{
			var argumentResults = methodCall.Arguments.Select(ExtractPaths);
			var argumentsResult = ExtractPathResult<Expression>.Flatten(argumentResults);
			var objectResult = methodCall.Object == null
				? new ExtractPathResult<Expression>(f => null)
				: ExtractPaths(methodCall.Object);
			return objectResult.Combine(argumentsResult, (newObject, newArguments) => (Expression)Expression.Call(newObject, methodCall.Method, newArguments));
		}

		private static Expression GetArrayParameter(ParameterExpression parameter, int index, Type type)
		{
			var arrayIndex = Expression.ArrayIndex(parameter, Expression.Constant(index));
			return Expression.Convert(arrayIndex, type);
		}
	}
}
