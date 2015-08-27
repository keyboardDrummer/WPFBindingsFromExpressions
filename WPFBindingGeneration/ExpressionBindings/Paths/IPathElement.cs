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

		string ToPathString();
		object Evaluate(object parameter);
	}
}
