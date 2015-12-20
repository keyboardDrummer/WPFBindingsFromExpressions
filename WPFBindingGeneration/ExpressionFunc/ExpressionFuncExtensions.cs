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

		public static IExpressionFunc<From, To> Compose<From, Mid, To>(this IExpressionFunc<From, Mid> inner, IExpressionFunc<Mid, To> outer)
		{
			var newOuterBody = new ReplaceParameter(outer.ExpressionTree.Parameters[0], inner.ExpressionTree.Body).Visit(outer.ExpressionTree.Body);
			return new ContextualExpression<From, To>(Expression.Lambda<Func<From, To>>(newOuterBody, inner.ExpressionTree.Parameters));
		}

		public static IExpressionFunc<From, To> Join<From, L, R, To>(IExpressionFunc<From, L> left, IExpressionFunc<From, R> right, Func<L, R, To> compose)
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
			if (parameter == null)
			{
				parameter = Expression.Parameter(typeof(Unit));
			}
			return new ContextualExpression<From, To>(Expression.Lambda<Func<From, To>>(convertBody, parameter));
		}

		static ParameterExpression GetParameter<From, L, R>(IExpressionFunc<From, L> left, IExpressionFunc<From, R> right)
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