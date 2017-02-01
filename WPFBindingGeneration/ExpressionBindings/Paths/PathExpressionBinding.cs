using System;
using System.Linq.Expressions;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public class PathExpressionBinding<From, To> : DefaultExpressionBinding<From, To>
	{
		public PathExpressionBinding(IPathExpression func)
		{
			this.Path = func;
		}

		public override bool IsWritable => Path.Writable;

		private object GetSource()
		{
			return Path.Source;
		}

		public override To Evaluate(From @from)
		{
			var obj = Path.Evaluate(from);
			if (obj == null)
			{
				return default(To);
			}

			return (To)obj;
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

		public override IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null)
		{
			return new ConvertedPathExpressionBinding<From, To, NewTo>(this, forward, backward);
		}
	}
}
