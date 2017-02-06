using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public interface IExpressionFunction<TTo>
	{
		LambdaExpression ExpressionTree
		{
			get;
		}

		Type ContextType
		{
			get;
		}

		IExpressionFunction<TNewTo> Convert<TNewTo>(Func<TTo, TNewTo> func);

		IExpressionBinding<TTo> ExpressionBinding
		{
			get;
		}
	}

	public interface IContextFreeExpression<TTo> : IExpressionFunction<TTo>
	{
		TTo Evaluate();
	}

	public interface IContextualExpression<in TFrom, TTo> : IExpressionFunction<TTo>
	{
		new IExpressionBinding<TFrom, TTo> ExpressionBinding
		{
			get;
		}

		TTo Evaluate(TFrom from);
	}
}
