using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WPFBindingGeneration.ExpressionBindings.Paths;

namespace WPFBindingGeneration
{
	delegate Expression CreatePathParameter(IPathExpression path, Type type);

	/// <summary>
	/// The result of extracting paths from an expression.
	/// </summary>
	class ExtractPathResult<T>
	{
		public ExtractPathResult(Func<CreatePathParameter, T> createExpression, params IPathExpression[] paths)
			: this(createExpression, new Utility.SortedSet<IPathExpression>(Comparer, paths))
		{
		}

		public ExtractPathResult(Func<CreatePathParameter, T> createExpression, Utility.SortedSet<IPathExpression> paths)
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

		public static ExtractPathResult<IEnumerable<T>> Flatten(IEnumerable<ExtractPathResult<T>> input)
		{
			var seed = new ExtractPathResult<IEnumerable<T>>(c => Enumerable.Empty<T>());
			return input.Aggregate(seed, (results, result) => results.Combine(result, (items, item) =>
			{
			    IEnumerable<T> enumerable = items.Concat(new[] {item});
			    return enumerable;
			}));
		}

		public ExtractPathResult<R> Combine<U, R>(ExtractPathResult<U> other, Func<T, U, R> merge)
		{
			var combinedPaths = new Utility.SortedSet<IPathExpression>(Comparer);
			foreach (var item in other.Paths.Concat(Paths))
				combinedPaths.Add(item);

			return new ExtractPathResult<R>(c => merge(CreateExpression(c), other.CreateExpression(c)), combinedPaths);
		}

		public ExtractPathResult<U> Select<U>(Func<T, U> func)
		{
			return new ExtractPathResult<U>(c => func(CreateExpression(c)), Paths);
		}
	}
}