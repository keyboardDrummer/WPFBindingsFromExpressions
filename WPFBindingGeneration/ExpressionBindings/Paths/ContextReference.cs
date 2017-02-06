using System;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class ContextReference : PathExpression
	{
		public ContextReference(object context, Type type)
		{
			Type = type;
			Context = context;
		}

		public object Context
		{
			get;
		}

		public override Type Type
		{
			get;
		}

		public override bool Writable => false;

		public override object Source => Context;

		public override PropertyPath ToPropertyPath()
		{
			return new PropertyPath("");
		}

		public override object Evaluate(object parameter)
		{
			return Context;
		}

		public override void Write(object @from, object newTo)
		{
			throw new NotSupportedException();
		}
	}
}
