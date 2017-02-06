using System;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings
{
	public abstract class DefaultExpressionBinding<From, To> : IExpressionBinding<From, To>
	{
		public abstract BindingBase ToBindingBase();
		public abstract To Evaluate(From @from);
		public abstract void Write(From @from, To newTo);

		public abstract IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null);

		IExpressionBinding<NewTo> IExpressionBinding<To>.Convert<NewTo>(Func<To, NewTo> forward, Func<NewTo, To> backward)
		{
			return Convert(forward, backward);
		}

		public abstract bool IsWritable { get; }

		public Type TargetType => typeof (To);

		public Type SourceType => typeof (From);
	}
}