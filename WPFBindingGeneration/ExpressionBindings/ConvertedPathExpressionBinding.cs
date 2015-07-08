using System;
using System.Windows.Data;
using WPFBindingGeneration.ExpressionBindings.Converters;
using WPFBindingGeneration.ExpressionBindings.Paths;

namespace WPFBindingGeneration.ExpressionBindings
{
	public class ConvertedPathExpressionBinding<From, OldTo, To> : DefaultExpressionBinding<From, To>
	{
		readonly Func<To, OldTo> backward;
		readonly Func<OldTo, To> forward;
		readonly PathExpressionBinding<From, OldTo> pathExpression;

		public ConvertedPathExpressionBinding(PathExpressionBinding<From, OldTo> pathExpression, Func<OldTo, To> forward, Func<To, OldTo> backward)
		{
			this.pathExpression = pathExpression;
			this.forward = forward;
			this.backward = backward;
		}

		public override bool IsWritable
		{
			get { return pathExpression.IsWritable && backward != null; }
		}

		public override To Evaluate(From @from)
		{
			return forward(pathExpression.Evaluate(from));
		}

		public override BindingBase ToBindingBase()
		{
			var result = pathExpression.ToBinding();
			result.Converter = new ValueConverterFromDelegate<OldTo, To>(forward, backward);
			return result;
		}

		public override object DataContext
		{
			get { return pathExpression.DataContext; }
		}

		public override IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward2 = null, Func<NewTo, To> backward2 = null)
		{
			var combinedBackward = backward == null || backward2 == null
				? (Func<NewTo, OldTo>) null
				: v => backward(backward2(v));
			var combinedForward = forward == null || forward2 == null
				? (Func<OldTo, NewTo>) null
				: u => forward2(forward(u));
			return new ConvertedPathExpressionBinding<From, OldTo, NewTo>(pathExpression, combinedForward, combinedBackward);
		}
	}
}