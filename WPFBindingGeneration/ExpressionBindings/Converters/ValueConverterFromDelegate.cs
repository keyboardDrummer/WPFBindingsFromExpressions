using System;
using System.Globalization;
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
			var result = forward((T) value);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return backward((U) value);
		}
	}
}