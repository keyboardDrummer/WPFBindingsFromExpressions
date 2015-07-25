using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public class ContextFreeExpression<To> : DefaultExpressionFunc<Unit, To>
	{
		readonly Expression<Func<To>> tree;

		public ContextFreeExpression(Expression<Func<To>> tree)
		{
			this.tree = tree;
		}

		public override IExpressionBinding<Unit, To> ExpressionBinding
		{
			get { return ExpressionToBindingParser.OneWay(tree); }
		}

		public override LambdaExpression ExpressionTree
		{
			get { return tree; }
		}

		public override Type ContextType
		{
			get { return null; }
		}

		public override IExpressionFunc<Unit, NewTo> Convert<NewTo>(Func<To, NewTo> func)
		{
			var call = ExpressionFuncExtensions.CreateCall(func, tree.Body);
			var newTree = Expression.Lambda<Func<NewTo>>(call, tree.Parameters);
			return new ContextFreeExpression<NewTo>(newTree);
		}

		public override To Evaluate(Unit @from)
		{
			return tree.Compile()();
		}
	}
}