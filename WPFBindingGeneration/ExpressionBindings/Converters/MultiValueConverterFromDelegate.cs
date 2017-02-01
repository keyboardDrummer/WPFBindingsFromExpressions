using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Converters
{
	class MultiValueConverterFromDelegate<To> : IMultiValueConverter
	{
		private readonly Type[] _types;
		readonly Func<object[], To> converter;

		public MultiValueConverterFromDelegate(Type[] types, Func<object[], To> converter)
		{
			_types = types;
			this.converter = converter;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			for (int index = 0; index < _types.Length; index++)
			{
				var type = _types[index];
				if (!type.IsInstanceOfType(values[index]))
				{
					return default(To);
				}
			}
			return converter(values);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
