using System;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings
{
	public abstract class DefaultExpressionBinding<From, To> : IExpressionBinding<From, To>
	{
		public abstract BindingBase ToBindingBase();
		public abstract To Evaluate(From @from);
		public abstract IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null);
		public abstract bool IsWritable { get; }

		public Type TargetType
		{
			get { return typeof (To); }
		}

		public Type SourceType
		{
			get { return typeof (From); }
		}
	}
}