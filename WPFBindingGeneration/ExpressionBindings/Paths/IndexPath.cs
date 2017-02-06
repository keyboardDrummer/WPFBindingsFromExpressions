using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	public class IndexPath : PathExpression
	{
		private readonly IPathExpression inner;
		private readonly MethodInfo _getter;
		private readonly MethodInfo _setter;
		private readonly object[] _indices;

		public IndexPath(IPathExpression inner, MethodInfo getter, MethodInfo setter, object[] indices)
		{
			this.inner = inner;
			this._getter = getter;
			_setter = setter;
			_indices = indices;
		}

		public override Type Type => _getter.ReturnType;

		public override bool Writable => _setter != null;

		public object Context
		{
			get;
			set;
		}

		public override object Source => Context;

		public override PropertyPath ToPropertyPath()
		{
			var innerPath = inner.ToPropertyPath();
			var innerParameters = innerPath.PathParameters;
			var allParameters = innerParameters.Concat(_indices).ToArray();
			var parametersString = string.Join(",", _indices.Select((_, indexIndex) => $"({innerParameters.Count + indexIndex})"));
			string path = $"{innerPath.Path}[{parametersString}]";
			return new PropertyPath(path, allParameters);
		}

		public override object Evaluate(object parameter)
		{
			var indexed = inner.Evaluate(parameter);
			if (indexed == null)
			{
				return null;
			}
			return _getter.Invoke(indexed, _indices);
		}

		public override void Write(object @from, object newTo)
		{
			var indexed = inner.Evaluate(@from);
			_setter.Invoke(indexed, _indices.Concat(new [] { newTo}).ToArray());
		}
	}
}