using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public class PropertyAccess : PathExpression
	{
		public PropertyAccess(PropertyInfo property, IPathExpression inner)
		{
			this.Property = property;
			this.Inner = inner;
		}

		public IPathExpression Inner
		{
			get;
		}

		public PropertyInfo Property
		{
			get;
		}

		public override Type Type => Property.PropertyType;

		public override bool Writable => Property.GetSetMethod(false) != null;

		public override object Source => Inner.Source;

		public override PropertyPath ToPropertyPath()
		{
			var innerPath = Inner.ToPropertyPath();
			var innerString = innerPath.Path;
			var separator = string.IsNullOrEmpty(innerString) || Inner is CurrentPath ? "" : ".";
			var prefix = innerString + separator;
			return new PropertyPath(prefix + Property.Name, innerPath.PathParameters?.ToArray());
		}

		public override object Evaluate(object parameter)
		{
			var innerValue = Inner.Evaluate(parameter);
			if (innerValue == null)
			{
				return null;
			}

			return Property.GetValue(innerValue);
		}

		public override void Write(object @from, object newTo)
		{
			Property.SetValue(Inner.Evaluate(from), newTo);
		}
	}
}
