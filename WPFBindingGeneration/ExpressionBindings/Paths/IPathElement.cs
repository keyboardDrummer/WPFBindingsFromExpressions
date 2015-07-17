namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public interface IPathElement
	{
		bool Writable { get; }
		string ToPathString();
		object Evaluate(object parameter);
	}
}