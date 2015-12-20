using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public abstract class DefaultExpressionFunc<From, To> : IExpressionFunc<From, To>
	{
		public abstract IExpressionFunc<From, NewTo> Convert<NewTo>(Func<To, NewTo> func);
		public abstract To Evaluate(From @from);
		public abstract LambdaExpression ExpressionTree { get; }
		public abstract Type ContextType { get; }
		public abstract IExpressionBinding<From, To> ExpressionBinding { get; }
	}
}