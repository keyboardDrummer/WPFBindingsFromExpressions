using System.Linq;
using System.Reflection;
using System.Windows;

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

		public bool Writable
		{
			get
			{
				if (property.Name.Contains("Header"))
				{
					int b = 0;
					b++;
				}
				return property.GetSetMethod(false) != null;
			}
		}

		public object Source => inner.Source;

		public PropertyPath ToPathString()
		{
			var innerPath = inner.ToPathString();
			var innerString = innerPath.Path;
			var separator = string.IsNullOrEmpty(innerString) || inner is CurrentPath ? "" : ".";
			var prefix = innerString + separator;
			return new PropertyPath(prefix + property.Name, innerPath.PathParameters?.ToArray());
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
