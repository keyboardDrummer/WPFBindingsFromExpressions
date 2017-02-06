using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public class ContextualExpression<TFrom, TTo> : IContextualExpression<TFrom, TTo>
	{
		readonly Expression<Func<TFrom, TTo>> tree;

		public ContextualExpression(Expression<Func<TFrom, TTo>> tree)
		{
			this.tree = tree;
		}

		public IExpressionBinding<TFrom, TTo> ExpressionBinding => ExpressionToBindingParser.OneWay(tree);

		public Type ContextType
		{
			get
			{
				var type = typeof(TFrom);
				return type == typeof(Unit) ? null : type;
			}
		}

		public LambdaExpression ExpressionTree => tree;

		public IContextualExpression<TFrom, TNewTo> Convert<TNewTo>(Func<TTo, TNewTo> func)
		{
			var call = ExpressionFuncExtensions.CreateCall(func, tree.Body);
			var newTree = Expression.Lambda<Func<TFrom, TNewTo>>(call, tree.Parameters);
			return new ContextualExpression<TFrom, TNewTo>(newTree);
		}

		IExpressionBinding<TTo> IExpressionFunction<TTo>.ExpressionBinding => ExpressionBinding;

		IExpressionFunction<TNewTo> IExpressionFunction<TTo>.Convert<TNewTo>(Func<TTo, TNewTo> func)
		{
			return Convert(func);
		}

		public TTo Evaluate(TFrom @from)
		{
			return tree.DebugCompile()(from);
		}
	}
}