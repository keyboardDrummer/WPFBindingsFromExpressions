using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using WPFBindingGeneration.ExpressionBindings.Converters;
using WPFBindingGeneration.ExpressionBindings.Paths;

namespace WPFBindingGeneration.ExpressionBindings
{
	class MultiPathExpressionBinding<From, To> : DefaultExpressionBinding<From, To>
	{
		readonly Func<To, object[]> backward;
		readonly Func<object[], To> forward;
		readonly IReadOnlyList<IPathExpression> paths;

		public MultiPathExpressionBinding(IReadOnlyList<IPathExpression> paths, Func<object[], To> forward, Func<To, object[]> backward)
		{
			this.paths = paths;
			this.forward = forward;
			this.backward = backward;
		}

		public override bool IsWritable
		{
			get { return paths.All(path => path.Writable) && backward != null; }
		}

		IEnumerable<PathExpressionBinding<From, object>> PathExpressionBindings
		{
			get { return paths.Select(path => new PathExpressionBinding<From, object>(path)); }
		}

		public override To Evaluate(From @from)
		{
			var pathValues = PathExpressionBindings.Select(path => path.Evaluate(from));
			return forward(pathValues.ToArray());
		}

		public override BindingBase ToBindingBase()
		{
			return ToBinding();
		}

		public MultiBinding ToBinding()
		{
			var multiBinding = new MultiBinding();
			foreach (var path in PathExpressionBindings)
			{
				multiBinding.Bindings.Add(path.ToBindingBase());
			}
			multiBinding.Mode = BindingMode.OneWay; 
			multiBinding.Converter = new MultiValueConverterFromDelegate<To>(forward);
			return multiBinding;
		}

		public override IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward2 = null, Func<NewTo, To> backward2 = null)
		{
			var combinedBackward = backward == null || backward2 == null
				? (Func<NewTo, object[]>) null
				: v => backward(backward2(v));
			var combinedForward = forward == null || forward2 == null
				? (Func<object[], NewTo>) null
				: u => forward2(forward(u));
			return new MultiPathExpressionBinding<From, NewTo>(paths, combinedForward, combinedBackward);
		}
	}
}