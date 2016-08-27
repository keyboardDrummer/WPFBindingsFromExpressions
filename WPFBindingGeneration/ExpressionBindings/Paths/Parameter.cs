using System.Linq.Expressions;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class Parameter : IPathElement
	{
		private readonly ParameterExpression parameterExpression;

		public Parameter(ParameterExpression parameterExpression)
		{
			this.parameterExpression = parameterExpression;
		}

		public bool Writable => false;

		public object Source => null;

		public PropertyPath ToPathString()
		{
			return new PropertyPath("");
		}

		public object Evaluate(object parameter)
		{
			return parameter;
		}
	}
}
