using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WPFBindingGeneration
{
	public static class ExpressionTreeExtensions
	{
		const bool UseDebugCompile = true;

		/// <summary>
		/// In release mode, returns <c>expr.Compile()</c>.
		/// In debug mode, wraps the compiled delegate in another delegate which
		/// also captures the expression <c>expr</c> so that when
		/// the compiled expression throws, we wrap that exception in
		/// another one along with a text representation of the original
		/// expression. This greatly helps in debugging where
		/// we now get messages like 'expression (MyClass x) => x.Y.Z'
		/// threw an exception instead of something like "nullptr at
		/// compiler generated method".
		/// </summary>
		public static Func<R> DebugCompile<R>(this Expression<Func<R>> expr)
		{
			if (UseDebugCompile)
			{
				var func = expr.Compile();
				return () =>
				{
					try
					{
						return func();
					}
					catch (Exception e)
					{
						throw new TargetInvocationException(string.Format("An exception occurred in compiled expression \"{0}\".", expr), e);
					}
				};
			}
			return expr.Compile();
		}

		/// <summary>
		/// In release mode, returns <c>expr.Compile()</c>.
		/// In debug mode, wraps the compiled delegate in another delegate which
		/// also captures the expression <c>expr</c> so that when
		/// the compiled expression throws, we wrap that exception in
		/// another one along with a text representation of the original
		/// expression. This greatly helps in debugging where
		/// we now get messages like 'expression (MyClass x) => x.Y.Z'
		/// threw an exception instead of something like "nullptr at
		/// compiler generated method".
		/// </summary>
		public static Func<T, R> DebugCompile<T, R>(this Expression<Func<T, R>> expr)
		{
			if (UseDebugCompile)
			{
				var func = expr.Compile();
				return x =>
				{
					try
					{
						return func(x);
					}
					catch (Exception e)
					{
						throw new TargetInvocationException(string.Format("An exception occurred in compiled expression \"{0}\".", expr), e);
					}
				};
			}
			return expr.Compile();
		}
	}
}