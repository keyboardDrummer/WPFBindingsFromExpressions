using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public class ContextFreeExpression<TTo> : IContextFreeExpression<TTo>
	{
		readonly Expression<Func<TTo>> tree;

		public ContextFreeExpression(Expression<Func<TTo>> tree)
		{
			this.tree = tree;
		}

		public LambdaExpression ExpressionTree => tree;

		public Type ContextType => null;

		public IExpressionFunction<TNewTo> Convert<TNewTo>(Func<TTo, TNewTo> func)
		{
			var call = ExpressionFuncExtensions.CreateCall(func, tree.Body);
			var newTree = Expression.Lambda<Func<TNewTo>>(call, tree.Parameters);
			return new ContextFreeExpression<TNewTo>(newTree);
		}
		
		public IExpressionBinding ExpressionBinding => ExpressionToBindingParser.OneWay(tree);

		public TTo Evaluate()
		{
			return tree.DebugCompile()();
		}
	}
}