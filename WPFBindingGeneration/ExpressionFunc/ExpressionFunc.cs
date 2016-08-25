using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public interface IExpressionFuncBase<out TTo>
	{
		LambdaExpression ExpressionTree
		{
			get;
		}

		Type ContextType
		{
			get;
		}

		IExpressionFuncBase<TNewTo> Convert<TNewTo>(Func<TTo, TNewTo> func);

		IExpressionBinding ExpressionBinding
		{
			get;
		}
	}

	public interface IExpressionFunc<TTo> : IExpressionFuncBase<TTo>
	{
		TTo Evaluate();
	}

	public interface IExpressionFunc<in TFrom, TTo> : IExpressionFuncBase<TTo>
	{
		new IExpressionBinding<TFrom, TTo> ExpressionBinding
		{
			get;
		}

		TTo Evaluate(TFrom from);
	}
}
