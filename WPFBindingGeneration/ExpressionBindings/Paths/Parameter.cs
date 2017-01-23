using System.Linq.Expressions;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class Parameter : PathExpression
	{
		private readonly ParameterExpression parameterExpression;

		public Parameter(ParameterExpression parameterExpression)
		{
			this.parameterExpression = parameterExpression;
		}

		public override bool Writable => false;

		public override object Source => null;

		public override PropertyPath ToPropertyPath()
		{
			return new PropertyPath("");
		}

		public override object Evaluate(object parameter)
		{
			return parameter;
		}
	}
}
