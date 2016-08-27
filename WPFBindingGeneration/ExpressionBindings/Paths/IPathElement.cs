using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public interface IPathElement
	{
		bool Writable
		{
			get;
		}

		object Source
		{
			get;
		}

		PropertyPath ToPathString();
		object Evaluate(object parameter);
	}
}
