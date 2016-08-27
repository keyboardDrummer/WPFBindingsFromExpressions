using System;
using System.Linq.Expressions;

namespace WPFBindingGeneration.ExpressionFunc
{
	public static class ExpressionFuncExtensions
	{
		public static IContextualExpression<TFrom, TTo> Create<TFrom, TTo>(Expression<Func<TFrom, TTo>> expression)
		{
			return new ContextualExpression<TFrom, TTo>(expression);
		}

		public static IContextFreeExpression<TTo> Create<TTo>(Expression<Func<TTo>> expression)
		{
			return new ContextFreeExpression<TTo>(expression);
		}

		internal static Expression CreateCall(Delegate compose, params Expression[] arguments)
		{
			return Expression.Invoke(Expression.Constant(compose), arguments);
		}

		public static IContextualExpression<TFrom, TTo> Compose<TFrom, TMid, TTo>(this IContextualExpression<TFrom, TMid> inner, IContextualExpression<TMid, TTo> outer)
		{
			var newOuterBody = new ReplaceParameter(outer.ExpressionTree.Parameters[0], inner.ExpressionTree.Body).Visit(outer.ExpressionTree.Body);
			return new ContextualExpression<TFrom, TTo>(Expression.Lambda<Func<TFrom, TTo>>(newOuterBody, inner.ExpressionTree.Parameters));
		}

		public static IExpressionFunction<TNewTo> Join<TRight, TNewTo, TLeft>(IExpressionFunction<TLeft> left, IExpressionFunction<TRight> right, 
			Func<TLeft, TRight, TNewTo> composition)
		{
			IExpressionFunction<TNewTo> expressionFuncBase;
			var cfLeft = left as IContextFreeExpression<TLeft>;
			if (cfLeft != null)
			{
				var cfRight = right as IContextFreeExpression<TRight>;
				if (cfRight != null)
				{
					expressionFuncBase = Join(cfLeft, cfRight, composition);
				}
				else
				{
					var cRight = (IContextualExpression<object, TRight>)right;
					expressionFuncBase = Join(cfLeft, cRight, composition);
				}
			}
			else
			{
				var cLeft = (IContextualExpression<object, TLeft>)left;
				var cfRight = right as IContextFreeExpression<TRight>;
				if (cfRight != null)
				{
					expressionFuncBase = Join(cLeft, cfRight, composition);
				}
				else
				{
					var cRight = (IContextualExpression<object, TRight>)right;
					expressionFuncBase = Join(cLeft, cRight, composition);
				}
			}
			return expressionFuncBase;
		}


		public static IContextFreeExpression<TTo> Join<TLeftMid, TRightMid, TTo>(
			IContextFreeExpression<TLeftMid> left, IContextFreeExpression<TRightMid> right, Func<TLeftMid, TRightMid, TTo> compose)
		{
			return new ContextFreeExpression<TTo>(Expression.Lambda<Func<TTo>>(CreateCall(compose, left.ExpressionTree.Body, right.ExpressionTree.Body)));
		}

		public static IContextualExpression<TFrom, TTo> Join<TFrom, TLeftMid, TRightMid, TTo>(
			IContextualExpression<TFrom, TLeftMid> left, IContextFreeExpression<TRightMid> right, Func<TLeftMid, TRightMid, TTo> compose)
		{
			var body = CreateCall(compose, left.ExpressionTree.Body, right.ExpressionTree.Body);
			return new ContextualExpression<TFrom, TTo>(Expression.Lambda<Func<TFrom, TTo>>(body, left.ExpressionTree.Parameters[0]));
		}

		public static IContextualExpression<TFrom, TTo> Join<TFrom, TLeftMid, TRightMid, TTo>(
			IContextFreeExpression<TLeftMid> left, IContextualExpression<TFrom, TRightMid> right, Func<TLeftMid, TRightMid, TTo> compose)
		{
			var body = CreateCall(compose, left.ExpressionTree.Body, right.ExpressionTree.Body);
			return new ContextualExpression<TFrom, TTo>(Expression.Lambda<Func<TFrom, TTo>>(body, right.ExpressionTree.Parameters[0]));
		}

		public static IContextualExpression<TFrom, TTo> Join<TFrom, TLeftMid, TRightMid, TTo>(
			IContextualExpression<TFrom, TLeftMid> left, IContextualExpression<TFrom, TRightMid> right, Func<TLeftMid, TRightMid, TTo> compose)
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

		static ParameterExpression GetParameter<TFrom, TLeftMid, TRightMid>(IContextualExpression<TFrom, TLeftMid> left, IContextualExpression<TFrom, TRightMid> right)
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