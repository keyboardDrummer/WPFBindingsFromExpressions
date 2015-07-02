using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFBindingGeneration.Bindings.Converters
{
	class TransformedMultiValueConverter<From, To> : IMultiValueConverter
	{
		readonly IMultiValueConverter converter;
		readonly Func<From, To> transformer;

		public TransformedMultiValueConverter(IMultiValueConverter converter, Func<From, To> transformer)
		{
			this.converter = converter;
			this.transformer = transformer;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return transformer((From) converter.Convert(values, targetType, parameter, culture));
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}