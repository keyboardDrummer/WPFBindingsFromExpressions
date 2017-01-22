using System.Linq;
using System.Reflection;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class PropertyAccess : IPathExpression
	{
		private readonly PropertyInfo property;
		private readonly IPathExpression inner;

		public PropertyAccess(PropertyInfo property, IPathExpression inner)
		{
			this.property = property;
			this.inner = inner;
		}

		public PropertyInfo Property => property;

		public bool Writable => property.GetSetMethod(false) != null;

		public object Source => inner.Source;

		public PropertyPath ToPropertyPath()
		{
			var innerPath = inner.ToPropertyPath();
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
