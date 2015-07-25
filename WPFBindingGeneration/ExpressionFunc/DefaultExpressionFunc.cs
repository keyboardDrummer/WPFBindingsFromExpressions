using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public abstract class DefaultExpressionFunc<From, To> : IExpressionFunc<From, To>
	{
		IExpressionBinding IExpressionFunc<To>.ExpressionBinding
		{
			get { return ExpressionBinding; }
		}

		public abstract IExpressionFunc<From, NewTo> Convert<NewTo>(Func<To, NewTo> func);

		To IExpressionFunc<To>.Evaluate(object @from)
		{
			return Evaluate((From) @from);
		}

		public abstract To Evaluate(From @from);

		IExpressionFunc<NewTo> IExpressionFunc<To>.Convert<NewTo>(Func<To, NewTo> func)
		{
			return Convert(func);
		}

		public abstract LambdaExpression ExpressionTree { get; }
		public abstract Type ContextType { get; }
		public abstract IExpressionBinding<From, To> ExpressionBinding { get; }
	}
}