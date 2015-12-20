using System;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFBindingGeneration.ExpressionFunc
{
	public class ContextualExpression<From, To> : DefaultExpressionFunc<From, To>
	{
		readonly Expression<Func<From, To>> tree;

		public ContextualExpression(Expression<Func<From, To>> tree)
		{
			this.tree = tree;
		}

		public override IExpressionBinding<From, To> ExpressionBinding
		{
			get { return ExpressionToBindingParser.OneWay(tree); }
		}

		public override Type ContextType
		{
			get
			{
				var type = typeof(From);
				return type == typeof(Unit) ? null : type;
			}
		}

		public override LambdaExpression ExpressionTree
		{
			get { return tree; }
		}

		public override IExpressionFunc<From, NewTo> Convert<NewTo>(Func<To, NewTo> func)
		{
			var call = ExpressionFuncExtensions.CreateCall(func, tree.Body);
			var newTree = Expression.Lambda<Func<From, NewTo>>(call, tree.Parameters);
			return new ContextualExpression<From, NewTo>(newTree);
		}

		public override To Evaluate(From @from)
		{
			return tree.DebugCompile()(from);
		}
	}
}