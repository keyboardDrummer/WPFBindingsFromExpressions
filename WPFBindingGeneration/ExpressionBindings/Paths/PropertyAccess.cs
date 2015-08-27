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

		public PropertyInfo Property
		{
			get
			{
				return property;
			}
		}

		public bool Writable
		{
			get
			{
				return property.GetSetMethod(false) != null;
			}
		}

		public object Source
		{
			get
			{
				return inner.Source;
			}
		}

		public string ToPathString()
		{
			return inner.ToPathString() + "." + property.Name;
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
