using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings.Paths;

namespace WPFBindingGeneration
{
	class ToStringEqualityComparer<T> : IEqualityComparer<T>
	{
		public bool Equals(T x, T y)
		{
			return x.ToString() == y.ToString();
		}

		public int GetHashCode(T obj)
		{
			return obj.ToString().GetHashCode();
		}
	}

	delegate Expression CreatePathParameter(IPathElement path, Type type);

	class ExtractPathsResult<T>
	{
		readonly Func<CreatePathParameter, T> createExpression;
		readonly Utility.SortedSet<IPathElement> paths;

		public ExtractPathsResult(Func<CreatePathParameter, T> createExpression, params IPathElement[] paths)
			: this(createExpression, new Utility.SortedSet<IPathElement>(Comparer, paths))
		{
		}

		public ExtractPathsResult(Func<CreatePathParameter, T> createExpression, Utility.SortedSet<IPathElement> paths)
		{
			this.paths = paths;
			this.createExpression = createExpression;
		}

		public static IEqualityComparer<IPathElement> Comparer => EqualityComparer<IPathElement>.Default;

		public Utility.SortedSet<IPathElement> Paths
		{
			get { return paths; }
		}

		public Func<CreatePathParameter, T> CreateExpression
		{
			get { return createExpression; }
		}

		public static ExtractPathsResult<IEnumerable<T>> Flatten(IEnumerable<ExtractPathsResult<T>> input)
		{
			var seed = new ExtractPathsResult<IEnumerable<T>>(c => Enumerable.Empty<T>());
			return input.Aggregate(seed, (results, result) => results.Combine(result, (items, item) =>
			{
			    IEnumerable<T> enumerable = items.Concat(new[] {item});
			    return enumerable;
			}));
		}

		public ExtractPathsResult<R> Combine<U, R>(ExtractPathsResult<U> other, Func<T, U, R> merge)
		{
			var combinedPaths = new Utility.SortedSet<IPathElement>(Comparer);
			foreach (var item in other.paths.Concat(paths))
				combinedPaths.Add(item);

			return new ExtractPathsResult<R>(c => merge(createExpression(c), other.createExpression(c)), combinedPaths);
		}

		public ExtractPathsResult<U> Select<U>(Func<T, U> func)
		{
			return new ExtractPathsResult<U>(c => func(createExpression(c)), paths);
		}
	}
}