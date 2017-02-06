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

	public interface IExpressionBinding<TTo> : IExpressionBinding
	{
		IExpressionBinding<NewTo> Convert<NewTo>(Func<TTo, NewTo> forward = null, Func<NewTo, TTo> backward = null);
	}

	/// <summary>
	/// Provides a thin layer over WPF bindings. Stores expression paths as expression trees instead of strings.
	/// Includes an evaluate method for targets other than WPF.
	/// </summary>
	public interface IExpressionBinding<in From, To> : IExpressionBinding<To>
	{
		To Evaluate(From from);
		void Write(From from, To newTo);
		IExpressionBinding<From, NewTo> Convert<NewTo>(Func<To, NewTo> forward = null, Func<NewTo, To> backward = null);
	}
}