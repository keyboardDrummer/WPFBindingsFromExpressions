using System;
using System.Linq;
using System.Linq.Expressions;

namespace WPFBindingGeneration.ExpressionFunc
{
	public static class ExpressionFuncExtensions
	{
		public static IExpressionFunc<From, To> Create<From, To>(Expression<Func<From, To>> expression)
		{
			return new ContextualExpression<From, To>(expression);
		}

		public static IExpressionFunc<Unit, To> Create<To>(Expression<Func<To>> expression)
		{
			return new ContextFreeExpression<To>(expression);
		}

		internal static Expression CreateCall(Delegate compose, params Expression[] arguments)
		{
			return Expression.Invoke(Expression.Constant(compose), arguments);
		}

		public static IExpressionFunc<To> Compose<L, R, To>(IExpressionFunc<L> left, IExpressionFunc<R> right, Func<L, R, To> compose)
		{
			var leftTree = left.ExpressionTree;
			var parameter = GetParameter(left, right);
			var newLeftBody = left.ContextType == null
				? leftTree.Body
				: new ReplaceParameter(leftTree.Parameters[0], parameter).Visit(leftTree.Body);

			var rightTree = right.ExpressionTree;
			var newRightBody = right.ContextType == null
				? rightTree.Body
				: new ReplaceParameter(rightTree.Parameters[0], parameter).Visit(rightTree.Body);

			var convertBody = CreateCall(compose, newLeftBody, newRightBody);
			var lambda = parameter == null ? Expression.Lambda(convertBody) : Expression.Lambda(convertBody, parameter);
			return CreateExpressionFunc<To>(lambda);
		}

		static IExpressionFunc<To> CreateExpressionFunc<To>(LambdaExpression lambda)
		{
			if (lambda.Parameters.Any())
			{
				var type = typeof (ContextualExpression<,>).MakeGenericType(lambda.Parameters[0].Type, typeof (To));
				return (IExpressionFunc<To>) type.GetConstructors()[0].Invoke(new object[] {lambda});
			}
			else
			{
				var type = typeof (ContextFreeExpression<>).MakeGenericType(typeof (To));
				return (IExpressionFunc<To>) type.GetConstructors()[0].Invoke(new object[] {lambda});
			}
		}

		static ParameterExpression GetParameter<L, R>(IExpressionFunc<L> left, IExpressionFunc<R> right)
		{
			if (left.ContextType != null && right.ContextType != null)
			{
				if (left.ContextType != right.ContextType)
				{
					throw new NotSupportedException();
				}

				return left.ExpressionTree.Parameters[0];
			}
			if (left.ContextType != null)
			{
				return left.ExpressionTree.Parameters[0];
			}
			if (right.ContextType != null)
			{
				return right.ExpressionTree.Parameters[0];
			}
			return null;
		}
	}
}