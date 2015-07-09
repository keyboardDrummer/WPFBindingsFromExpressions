using System;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings
{
	public interface IExpressionBinding
	{
		bool IsWritable { get; }
		Type TargetType { get; }

		/// <summary>
		/// Returning null implies the data context is set by the container.
		/// </summary>
		object GetDataContext();

		BindingBase ToBindingBase();
	}

	/// <summary>
	/// Provides a thin layer over WPF bindings. Replaced the String expression paths with expression trees.
	/// Includes an evaluate method for targets other than WPF.
	/// </summary>
	public interface IExpressionBinding<in From, To> : IExpressionBinding
	{
		To Evaluate(From from);
		IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null);
	}
}