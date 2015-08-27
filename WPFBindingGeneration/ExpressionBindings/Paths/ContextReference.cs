namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class ContextReference : IPathElement
	{
		private readonly object context;

		public ContextReference(object context)
		{
			this.context = context;
		}

		public object Context
		{
			get
			{
				return context;
			}
		}

		public bool Writable
		{
			get
			{
				return false;
			}
		}

		public object Source
		{
			get
			{
				return context;
			}
		}

		public string ToPathString()
		{
			return "";
		}

		public object Evaluate(object parameter)
		{
			return context;
		}
	}
}
