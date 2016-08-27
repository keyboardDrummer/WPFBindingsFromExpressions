using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
	class IndexPath : IPathElement
	{
		private readonly IPathElement inner;
		private readonly MethodInfo _getter;
		private readonly MethodInfo _setter;
		private readonly object[] _indices;

		public IndexPath(IPathElement inner, MethodInfo getter, MethodInfo setter, object[] indices)
		{
			this.inner = inner;
			this._getter = getter;
			_setter = setter;
			_indices = indices;
		}

		public bool Writable => _setter != null;

		public object Context
		{
			get;
			set;
		}

		public object Source => Context;

		public PropertyPath ToPathString()
		{
			var innerPath = inner.ToPathString();
			var innerParameters = innerPath.PathParameters;
			var allParameters = innerParameters.Concat(_indices).ToList();
			string path = $"{innerPath.Path}[{string.Join(",", _indices.Select((_, indexIndex) => $"({innerParameters.Count + indexIndex})"))}]";
			return new PropertyPath(path, allParameters);
		}

		public object Evaluate(object parameter)
		{
			var indexed = inner.Evaluate(parameter);
			if (indexed == null)
			{
				return null;
			}
			return _getter.Invoke(indexed, _indices);
		}
	}
}