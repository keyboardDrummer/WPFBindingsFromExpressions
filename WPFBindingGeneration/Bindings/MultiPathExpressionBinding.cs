using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;
using WPFBindingGeneration.Bindings.Converters;
using WPFBindingGeneration.Bindings.Paths;

namespace WPFBindingGeneration.Bindings
{
	class MultiPathExpressionBinding<From, To> : DefaultExpressionBinding<From, To>
	{
		readonly Func<object[], To> converter;
		readonly IList<LambdaExpression> paths;

		public MultiPathExpressionBinding(IList<LambdaExpression> paths, Func<object[], To> converter)
		{
			this.paths = paths;
			this.converter = converter;
		}

		public override bool IsWritable
		{
			get { return false; }
		}

		public override To Evaluate(From @from)
		{
			var pathValues = paths.Select(path => path.Compile().DynamicInvoke(from));
			return converter(pathValues.ToArray());
		}

		public override BindingBase ToBindingBase()
		{
			return ToBinding();
		}

		public MultiBinding ToBinding()
		{
			var multiBinding = new MultiBinding();
			foreach (var path in paths)
			{
				multiBinding.Bindings.Add(new PathExpressionBinding<From, object>(path).ToBindingBase());
			}
			multiBinding.Converter = new MultiValueConverterFromDelegate<To>(converter);
			return multiBinding;
		}

		public MultiPathExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward)
		{
			return new MultiPathExpressionBinding<From, NewTo>(paths, inputs => forward(converter(inputs)));
		}
	}
}