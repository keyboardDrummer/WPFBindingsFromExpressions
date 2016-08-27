using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public abstract class DefaultExpressionFunc<TFrom, TTo> : IContextualExpression<TFrom, TTo>
	{
		public abstract TTo Evaluate(TFrom @from);
		public abstract LambdaExpression ExpressionTree { get; }
		public abstract Type ContextType { get; }
		public abstract IExpressionFunction<TNewTo> Convert<TNewTo>(Func<TTo, TNewTo> func);
		IExpressionBinding IExpressionFunction<TTo>.ExpressionBinding => ExpressionBinding;
		public abstract IExpressionBinding<TFrom, TTo> ExpressionBinding { get; }
	}
}