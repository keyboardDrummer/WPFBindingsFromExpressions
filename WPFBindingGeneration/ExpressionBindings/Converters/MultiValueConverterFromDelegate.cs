using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Converters
{
	class MultiValueConverterFromDelegate<To> : IMultiValueConverter
	{
		readonly Func<object[], To> converter;

		public MultiValueConverterFromDelegate(Func<object[], To> converter)
		{
			this.converter = converter;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				return converter(values);
			}
			catch (InvalidCastException)
			{
				if (values.Contains(DependencyProperty.UnsetValue))
					return null;
				throw;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}