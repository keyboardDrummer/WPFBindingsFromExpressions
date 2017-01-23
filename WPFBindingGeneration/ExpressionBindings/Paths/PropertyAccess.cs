using System.Linq;
using System.Reflection;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class PropertyAccess : PathExpression
	{
		private readonly IPathExpression inner;

		public PropertyAccess(PropertyInfo property, IPathExpression inner)
		{
			this.Property = property;
			this.inner = inner;
		}

		public PropertyInfo Property
		{
			get;
		}

		public override bool Writable => Property.GetSetMethod(false) != null;

		public override object Source => inner.Source;

		public override PropertyPath ToPropertyPath()
		{
			var innerPath = inner.ToPropertyPath();
			var innerString = innerPath.Path;
			var separator = string.IsNullOrEmpty(innerString) || inner is CurrentPath ? "" : ".";
			var prefix = innerString + separator;
			return new PropertyPath(prefix + Property.Name, innerPath.PathParameters?.ToArray());
		}

		public override object Evaluate(object parameter)
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
