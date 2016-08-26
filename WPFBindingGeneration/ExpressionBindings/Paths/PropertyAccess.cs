using System.Reflection;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class PropertyAccess : IPathElement
	{
		private readonly PropertyInfo property;
		private readonly IPathElement inner;

		public PropertyAccess(PropertyInfo property, IPathElement inner)
		{
			this.property = property;
			this.inner = inner;
		}

		public PropertyInfo Property => property;

		public bool Writable => property.GetSetMethod(false) != null;

		public object Source => inner.Source;

		public string ToPathString()
		{
			var innerString = inner.ToPathString();
			return (string.IsNullOrEmpty(innerString) ? "" : (innerString + ".")) + property.Name;
		}

		public object Evaluate(object parameter)
		{
			var innerValue = inner.Evaluate(parameter);
			if (innerValue == null)
			{
				return null;
			}

			return Property.GetValue(innerValue);
		}
	}
}
