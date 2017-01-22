using System;
using System.Windows.Data;

namespace WPFBindingGeneration.ExpressionBindings
{
	public interface IExpressionBinding
	{
		bool IsWritable { get; }
		Type TargetType { get; }
		Type SourceType { get; }
		BindingBase ToBindingBase();
	}

	/// <summary>
	/// Provides a thin layer over WPF bindings. Stores expression paths as expression trees instead of strings.
	/// Includes an evaluate method for targets other than WPF.
	/// </summary>
	public interface IExpressionBinding<in From, To> : IExpressionBinding
	{
		To Evaluate(From from);
		IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null);
	}
}