﻿using System;
using System.Linq.Expressions;
using System.Windows;
using WPFBindingGeneration.ExpressionBindings.Paths;

namespace WPFBindingGeneration.ExpressionBindings
{
	public static class ExpressionBindings
	{
		public static void Check(this IExpressionBinding binding)
		{
		}

		public static void Apply(this IExpressionBinding binding, FrameworkElement frameworkElement, DependencyProperty dependencyProperty)
		{
			//if (binding.TargetType != dependencyProperty.PropertyType)
			//	throw new ArgumentException();

			frameworkElement.SetBinding(dependencyProperty, binding.ToBindingBase());
		}

		public static PathExpressionBinding<From, To> Path<From, To>(Expression<Func<From, To>> func)
		{
			return new PathExpressionBinding<From, To>(PathExpressions.ParsePath(func));
		}

		public static IExpressionBinding<From, To> Convert<From, To>(Func<From, To> func)
		{
			return Root<From>().Convert(func);
		}

		public static PathExpressionBinding<From, From> Root<From>()
		{
			return Path((From x) => x);
		}
	}
}