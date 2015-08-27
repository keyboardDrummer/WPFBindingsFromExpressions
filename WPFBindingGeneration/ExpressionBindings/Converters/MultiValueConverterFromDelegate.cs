using System;
using System.Collections.Generic;
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
			if (values.All(x => x == DependencyProperty.UnsetValue))
			{
				return DependencyProperty.UnsetValue;
			}
			ISet<int> nulledElements = new HashSet<int>(); //TODO remove this debug info.
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == DependencyProperty.UnsetValue)
			{
					values[i] = null;
					nulledElements.Add(i);
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