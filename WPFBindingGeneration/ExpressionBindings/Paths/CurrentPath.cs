using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class CurrentPath : IPathExpression
	{
		private readonly IPathExpression inner;

		public CurrentPath(IPathExpression inner)
		{
			this.inner = inner;
		}

		public bool Writable => false;

		public object Source => inner.Source;

		public PropertyPath ToPropertyPath()
		{
			var innerPath = inner.ToPropertyPath();
			var innerString = innerPath.Path;
			return new PropertyPath(string.IsNullOrEmpty(innerString) ? "" : innerString + "/", innerPath.PathParameters?.ToArray());
		}

		public object Evaluate(object parameter)
		{
			return ((IEnumerable<object>)inner.Evaluate(parameter)).Current();
		}
	}
}