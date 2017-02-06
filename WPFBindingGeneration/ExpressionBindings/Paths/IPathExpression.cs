using System;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public abstract class PathExpression : IPathExpression
	{
		public abstract Type Type
		{
			get;
		}

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
		public abstract void Write(object @from, object newTo);

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
		Type Type
		{
			get;
		}

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
		void Write(object @from, object newTo);
	}
}
