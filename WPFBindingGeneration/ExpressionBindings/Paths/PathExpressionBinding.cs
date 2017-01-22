using System;
using System.Linq.Expressions;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public class PathExpressionBinding<From, To> : DefaultExpressionBinding<From, To>
	{
		private readonly IPathElement func;

		public PathExpressionBinding(IPathElement func)
		{
			this.func = func;
		}

		public override bool IsWritable => GetPath().Writable;

		private object GetSource()
		{
			return GetPath().Source;
		}

		public override To Evaluate(From @from)
		{
			var obj = GetPath().Evaluate(from);
			if (obj == null)
			{
				return default(To);
			}

			return (To)obj;
		}

		public Binding ToBinding()
		{
			var path = GetPath().ToPathString();
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

		private IPathElement GetPath()
		{
			return func;
		}

		public override IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null)
		{
			return new ConvertedPathExpressionBinding<From, To, NewTo>(this, forward, backward);
		}
	}
}
