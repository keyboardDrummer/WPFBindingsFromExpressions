using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class CurrentPath : IPathElement
	{
		private readonly IPathElement inner;

		public CurrentPath(IPathElement inner)
		{
			this.inner = inner;
		}

		public bool Writable => false;

		public object Source => inner.Source;

		public PropertyPath ToPathString()
		{
			var innerPath = inner.ToPathString();
			var innerString = innerPath.Path;
			return new PropertyPath(string.IsNullOrEmpty(innerString) ? "" : innerString + "/", innerPath.PathParameters?.ToArray());
		}

		public object Evaluate(object parameter)
		{
			return ((IEnumerable<object>)inner.Evaluate(parameter)).Current();
		}
	}
}