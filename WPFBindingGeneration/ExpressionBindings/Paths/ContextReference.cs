namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class ContextReference : IPathElement
	{
		readonly object context;

		public ContextReference(object context)
		{
			this.context = context;
		}

		public object Context
		{
			get { return context; }
		}

		public bool Writable
		{
			get { return false; }
		}

		public string ToPathString()
		{
			return "";
		}
	}
}