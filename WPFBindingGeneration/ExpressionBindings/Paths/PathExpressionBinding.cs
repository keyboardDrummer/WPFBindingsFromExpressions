using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public class PathExpressionBinding<From, To> : DefaultExpressionBinding<From, To>
	{
		readonly LambdaExpression func;

		public PathExpressionBinding(LambdaExpression func)
		{
			this.func = func;
		}

		public override bool IsWritable
		{
			get { return GetElements().Last().Writable; }
		}

		object GetSource()
		{
			var firstElement = GetElements().First() as ContextReference;
			return firstElement == null ? null : firstElement.Context;
		}

		public override To Evaluate(From @from)
		{
			var obj = GetElements().Aggregate((object) @from, (value, link) => link.Evaluate(value));
			if (obj == null)
				return default(To);

			return (To) obj;
		}

		public Binding ToBinding()
		{
			var result = new Binding(string.Join(".", GetElements().Select(element => element.ToPathString()).Where(s => !string.IsNullOrEmpty(s))));
			result.Mode = IsWritable ? BindingMode.TwoWay : BindingMode.OneWay;
			result.Source = GetSource();
			result.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			return result;
		}

		public override BindingBase ToBindingBase()
		{
			return ToBinding();
		}

		IEnumerable<IPathElement> GetElements()
		{
			return PathExpressions.GetPathElements(func.Body);
		}

		public override IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null)
		{
			return new ConvertedPathExpressionBinding<From, To, NewTo>(this, forward, backward);
		}
	}
}