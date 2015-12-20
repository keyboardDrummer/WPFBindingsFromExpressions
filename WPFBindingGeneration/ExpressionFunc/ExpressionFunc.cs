using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public interface IExpressionFunc<in From, To>
	{
		LambdaExpression ExpressionTree
		{
			get;
		}

		Type ContextType
		{
			get;
		}

		IExpressionBinding<From, To> ExpressionBinding
		{
			get;
		}

		IExpressionFunc<From, NewTo> Convert<NewTo>(Func<To, NewTo> func);
		To Evaluate(From from);
	}
}
