﻿using System;
using System.Linq.Expressions;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
	class TwoPathExpressionBinding<From, First, Second, To> : DefaultExpressionBinding
	{
		readonly Func<First, Second, To> converter;
		readonly Expression<Func<From, First>> first;
		readonly Expression<Func<From, Second>> second;

		public TwoPathExpressionBinding(Expression<Func<From, First>> first, Expression<Func<From, Second>> second, Func<First, Second, To> converter)
		{
			this.first = first;
			this.second = second;
			this.converter = converter;
		}

		public override bool IsWritable
		{
			get { return false; }
		}

		public override BindingBase ToBindingBase()
		{
			var multiBinding = new MultiBinding();
			multiBinding.Bindings.Add(new PathExpressionBinding<From, First>(first).ToBindingBase());
			multiBinding.Bindings.Add(new PathExpressionBinding<From, Second>(second).ToBindingBase());
			multiBinding.Converter = new MultiValueConverterFromDelegate<To>(values => converter((First) values[0], (Second) values[1]));
			return multiBinding;
		}
	}
}