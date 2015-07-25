using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public interface IExpressionFunc<out To>
	{
		LambdaExpression ExpressionTree { get; }
		Type ContextType { get; }
		IExpressionBinding ExpressionBinding { get; }
		IExpressionFunc<NewTo> Convert<NewTo>(Func<To, NewTo> func);
		To Evaluate(object from);
	}

	public interface IExpressionFunc<in From, To> : IExpressionFunc<To>
	{
		new IExpressionBinding<From, To> ExpressionBinding { get; }
		new IExpressionFunc<From, NewTo> Convert<NewTo>(Func<To, NewTo> func);
		To Evaluate(From from);
	}
}