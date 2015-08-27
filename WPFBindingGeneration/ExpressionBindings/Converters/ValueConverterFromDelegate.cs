using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Converters
{
	class ValueConverterFromDelegate<T, U> : IValueConverter
	{
		readonly Func<U, T> backward;
		readonly Func<T, U> forward;

		public ValueConverterFromDelegate(Func<T, U> forward, Func<U, T> backward)
		{
			this.forward = forward;
			this.backward = backward;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == DependencyProperty.UnsetValue || (value != null && !(value is T)))
			{
				return DependencyProperty.UnsetValue;
			}
			return forward(value == null ? default(T) : (T)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (backward == null)
			{
				throw new NotSupportedException(string.Format(
					"DelegateConverter does not support two way bindings: source type = {0}, target type = {1}.",
					typeof(T).Name, typeof(U).Name));
			}
			if (value == DependencyProperty.UnsetValue || (value != null && !(value is U)))
			{
				return DependencyProperty.UnsetValue;
			}
			return backward((U) value);
		}
	}
}