using System.Linq.Expressions;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class Parameter : IPathExpression
	{
		private readonly ParameterExpression parameterExpression;

		public Parameter(ParameterExpression parameterExpression)
		{
			this.parameterExpression = parameterExpression;
		}

		public bool Writable => false;

		public object Source => null;

		public PropertyPath ToPropertyPath()
		{
			return new PropertyPath("");
		}

		public object Evaluate(object parameter)
		{
			return parameter;
		}
	}
}
