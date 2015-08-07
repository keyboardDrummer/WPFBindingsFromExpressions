using System.Collections;
using System.Collections.Generic;

namespace WPFBindingGeneration.Utility
{
	class SortedSet<T> : IReadOnlyList<T>
	{
		readonly IList<T> list = new List<T>();
		readonly ISet<T> set;

		public SortedSet(IEqualityComparer<T> comparer)
		{
			set = new HashSet<T>(comparer);
		}

		public SortedSet(IEqualityComparer<T> comparer, params T[] items) : this(comparer)
		{
			foreach (var item in items)
				Add(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return list.Count; }
		}

		public T this[int index]
		{
			get { return list[index]; }
		}

		public bool Add(T item)
		{
			if (set.Add(item))
			{
				list.Add(item);
				return true;
			}
			return false;
		}
	}
}