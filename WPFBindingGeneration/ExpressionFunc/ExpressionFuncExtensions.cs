using System;
using System.Linq.Expressions;

namespace WPFBindingGeneration.ExpressionFunc
{
	public static class ExpressionFuncExtensions
	{
		public static IExpressionFunc<TFrom, TTo> Create<TFrom, TTo>(Expression<Func<TFrom, TTo>> expression)
		{
			return new ContextualExpression<TFrom, TTo>(expression);
		}

		public static IExpressionFunc<TTo> Create<TTo>(Expression<Func<TTo>> expression)
		{
			return new ContextFreeExpression<TTo>(expression);
		}

		internal static Expression CreateCall(Delegate compose, params Expression[] arguments)
		{
			return Expression.Invoke(Expression.Constant(compose), arguments);
		}

		public static IExpressionFunc<TFrom, TTo> Compose<TFrom, TMid, TTo>(this IExpressionFunc<TFrom, TMid> inner, IExpressionFunc<TMid, TTo> outer)
		{
			var newOuterBody = new ReplaceParameter(outer.ExpressionTree.Parameters[0], inner.ExpressionTree.Body).Visit(outer.ExpressionTree.Body);
			return new ContextualExpression<TFrom, TTo>(Expression.Lambda<Func<TFrom, TTo>>(newOuterBody, inner.ExpressionTree.Parameters));
		}

		public static IExpressionFunc<TFrom, TTo> Join<TFrom, TLeftMid, TRightMid, TTo>(
			IExpressionFunc<TFrom, TLeftMid> left, IExpressionFunc<TFrom, TRightMid> right, Func<TLeftMid, TRightMid, TTo> compose)
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
			return new ContextualExpression<TFrom, TTo>(Expression.Lambda<Func<TFrom, TTo>>(convertBody, parameter));
		}

		static ParameterExpression GetParameter<TFrom, TLeftMid, TRightMid>(IExpressionFunc<TFrom, TLeftMid> left, IExpressionFunc<TFrom, TRightMid> right)
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