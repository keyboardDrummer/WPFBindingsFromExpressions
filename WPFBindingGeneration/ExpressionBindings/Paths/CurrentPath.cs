using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class CurrentPath : PathExpression
	{
		private readonly IPathExpression inner;

		public CurrentPath(IPathExpression inner)
		{
			this.inner = inner;
		}

		public override Type Type => inner.Type.GetGenericArguments()[0];

		public override bool Writable => false;

		public override object Source => inner.Source;

		public override PropertyPath ToPropertyPath()
		{
			var innerPath = inner.ToPropertyPath();
			var innerString = innerPath.Path;
			return new PropertyPath(innerString + "/", innerPath.PathParameters?.ToArray());
		}

		public override object Evaluate(object parameter)
		{
			return ((IEnumerable<object>)inner.Evaluate(parameter)).Current();
		}

		public override void Write(object @from, object newTo)
		{
			throw new NotSupportedException();
		}
	}
}