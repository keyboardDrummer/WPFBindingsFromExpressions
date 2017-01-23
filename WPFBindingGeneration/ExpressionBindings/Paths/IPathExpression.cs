using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	abstract class PathExpression : IPathExpression
	{
		public abstract bool Writable
		{
			get;
		}

		public abstract object Source
		{
			get;
		}

		public abstract PropertyPath ToPropertyPath();
		public abstract object Evaluate(object parameter);

		public override string ToString()
		{
			return ToPropertyPath().Path;
		}
	}

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
