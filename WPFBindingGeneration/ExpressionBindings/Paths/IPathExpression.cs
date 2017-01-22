using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	/// <summary>
	/// An expression that has the form of a path, such as A.B.C
	/// </summary>
	public interface IPathExpression
	{
		bool Writable
		{
			get;
		}

		object Source
		{
			get;
		}

		PropertyPath ToPropertyPath();

		object Evaluate(object parameter);
	}
}
