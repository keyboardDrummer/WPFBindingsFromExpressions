﻿using System;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators.Bindings
{
	public class ConvertedPathExpressionBinding<From, OldTo, To> : DefaultExpressionBinding
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

		public override BindingBase ToBindingBase()
		{
			var result = pathExpression.ToBinding();
			result.Converter = new DelegateConverter<OldTo, To>(forward, backward);
			return result;
		}

		public ConvertedPathExpressionBinding<From, OldTo, NewTo> Convert<NewTo>(Func<To, NewTo> forward2, Func<NewTo, To> backward2 = null)
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