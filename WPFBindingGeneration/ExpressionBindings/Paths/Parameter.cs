using System.Linq.Expressions;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class Parameter : IPathElement
	{
		private readonly ParameterExpression parameterExpression;

		public Parameter(ParameterExpression parameterExpression)
		{
			this.parameterExpression = parameterExpression;
		}

		public bool Writable
		{
			get
			{
				return false;
			}
		}

		public object Source
		{
			get
			{
				return null;
			}
		}

		public string ToPathString()
		{
			return "";
		}

		public object Evaluate(object parameter)
		{
			return parameter;
		}
	}
}
