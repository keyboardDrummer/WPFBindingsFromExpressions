using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class ContextReference : IPathElement
	{
		public ContextReference(object context)
		{
			Context = context;
		}

		public object Context
		{
			get;
		}

		public bool Writable => false;

		public object Source => Context;

		public PropertyPath ToPathString()
		{
			return new PropertyPath("");
		}

		public object Evaluate(object parameter)
		{
			return Context;
		}
	}
}
