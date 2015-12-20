using System.Linq.Expressions;

namespace WPFBindingGeneration.ExpressionFunc
{
	class ReplaceParameter : ExpressionVisitor
	{
		readonly ParameterExpression from;
		readonly Expression to;

		public ReplaceParameter(ParameterExpression @from, Expression to)
		{
			this.@from = @from;
			this.to = to;
		}

		public override Expression Visit(Expression node)
		{
			if (@from == to)
				return node;

			return base.Visit(node);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == @from)
				return to;
			return base.VisitParameter(node);
		}
	}
}