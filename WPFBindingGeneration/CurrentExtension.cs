using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace WPFBindingGeneration
{
	/// <summary>
	/// Contains static utility methods for dealing with databinding to collections.
	/// </summary>
	public static class CurrentExtension
	{
		/// <summary>
		/// Gets the collection's current item.
		/// </summary>
		public static T Current<T>(this IEnumerable<T> enumerable)
		{
			return enumerable != null && enumerable.Any() 
				? (T)CollectionViewSource.GetDefaultView(enumerable).CurrentItem 
				: default(T);
		}

		/// <summary>
		/// Adds an eventhandler to the given collection's currency manager which will be notified when the current item changes.
		/// </summary>
		public static void AddCurrentChangedHandler<T>(this IEnumerable<T> enumerable, EventHandler handler)
		{
			CollectionViewSource.GetDefaultView(enumerable).CurrentChanged += handler;
		}

		/// <summary>
		/// Removes an event handler which was previously added using <c>AddCurrentChangedHandler</c>.
		/// </summary>
		public static void RemoveCurrentChangedHandler<T>(this IEnumerable<T> enumerable, EventHandler handler)
		{
			CollectionViewSource.GetDefaultView(enumerable).CurrentChanged -= handler;
		}
	}
}
