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

	delegate Expression CreatePathParameter(IPathExpression path, Type type);

	/// <summary>
	/// Contains a set of paths and a function to build a T that takes the results of those paths as input.
	/// </summary>
	class PathsAndReduceExpression<T>
	{
		public PathsAndReduceExpression(Func<CreatePathParameter, T> createExpression, params IPathExpression[] paths)
			: this(createExpression, new Utility.SortedSet<IPathExpression>(Comparer, paths))
		{
		}

		public PathsAndReduceExpression(Func<CreatePathParameter, T> createExpression, Utility.SortedSet<IPathExpression> paths)
		{
			Paths = paths;
			CreateExpression = createExpression;
		}

		public static IEqualityComparer<IPathExpression> Comparer => EqualityComparer<IPathExpression>.Default;

		public Utility.SortedSet<IPathExpression> Paths
		{
			get;
		}

		public Func<CreatePathParameter, T> CreateExpression
		{
			get;
		}

		public static PathsAndReduceExpression<IEnumerable<T>> Flatten(IEnumerable<PathsAndReduceExpression<T>> input)
		{
			var seed = new PathsAndReduceExpression<IEnumerable<T>>(c => Enumerable.Empty<T>());
			return input.Aggregate(seed, (results, result) => results.Combine(result, (items, item) =>
			{
			    IEnumerable<T> enumerable = items.Concat(new[] {item});
			    return enumerable;
			}));
		}

		public PathsAndReduceExpression<R> Combine<U, R>(PathsAndReduceExpression<U> other, Func<T, U, R> merge)
		{
			var combinedPaths = new Utility.SortedSet<IPathExpression>(Comparer);
			foreach (var item in other.Paths.Concat(Paths))
				combinedPaths.Add(item);

			return new PathsAndReduceExpression<R>(c => merge(CreateExpression(c), other.CreateExpression(c)), combinedPaths);
		}

		public PathsAndReduceExpression<U> Select<U>(Func<T, U> func)
		{
			return new PathsAndReduceExpression<U>(c => func(CreateExpression(c)), Paths);
		}
	}
}