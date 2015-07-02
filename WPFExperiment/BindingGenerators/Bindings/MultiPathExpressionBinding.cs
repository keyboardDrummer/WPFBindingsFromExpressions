using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
	class MultiPathExpressionBinding<From, To> : DefaultExpressionBinding
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