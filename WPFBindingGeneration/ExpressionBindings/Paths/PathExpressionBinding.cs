using System;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public class PathExpressionBinding<TFrom, TTo> : DefaultExpressionBinding<TFrom, TTo>
	{
		public PathExpressionBinding(IPathExpression func)
		{
			Path = func;
		}

		public override bool IsWritable => Path.Writable;

		private object GetSource()
		{
			return Path.Source;
		}

		public override TTo Evaluate(TFrom @from)
		{
			var obj = Path.Evaluate(from);
			if (obj == null)
			{
				return default(TTo);
			}

			return (TTo)obj;
		}

		public override void Write(TFrom @from, TTo newTo)
		{
			Path.Write(from, newTo);
		}

		public Binding ToBinding()
		{
			var path = Path.ToPropertyPath();
			var result = new Binding();
			if (!string.IsNullOrEmpty(path.Path))
			{
				result.Path = path;
			}
			result.Mode = IsWritable ? BindingMode.TwoWay : BindingMode.OneWay;
			var source = GetSource();
			if (source != null)
			{
				result.Source = source;
			}
			result.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			return result;
		}

		public override BindingBase ToBindingBase()
		{
			return ToBinding();
		}

		public IPathExpression Path
		{
			get;
		}

		public override IExpressionBinding<TFrom, NewTo> Convert<NewTo>(Func<TTo, NewTo> forward = null, Func<NewTo, TTo> backward = null)
		{
			return new ConvertedPathExpressionBinding<TFrom, TTo, NewTo>(this, forward, backward);
		}
	}
}
