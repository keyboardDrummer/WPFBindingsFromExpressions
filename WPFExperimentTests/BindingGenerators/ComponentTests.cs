using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using WPFBindingGeneration;
using WPFBindingGeneration.ExpressionBindings;
using Xunit;

namespace WPFExperimentTests.BindingGenerators
{
	public class ComponentTests
	{
		ISet<Item> items;
		public string SomeText { get; set; }

		[Fact]
		public void SinglePathNoConverter()
		{
			var singlePath = ExpressionToBindingParser.TwoWay((Item item) => item.IsChecked);
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
		}

		[Fact]
		public void SinglePathWithNotConverter()
		{
			var singlePath = ExpressionToBindingParser.OneWay((Item item) => !item.IsChecked);
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(false, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void SinglePathWithBinaryConverter()
		{
			var singlePath = ExpressionToBindingParser.OneWay((Item item) => item.IsChecked || true);
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(true, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void MultiplePathWithAnd()
		{
			var singlePath = ExpressionToBindingParser.OneWay((Item item) => item.IsChecked && item.ChildItem.IsChecked);
			var binding = (MultiBinding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", ((Binding) binding.Bindings[0]).Path.Path);
			Assert.Equal("ChildItem.IsChecked", ((Binding) binding.Bindings[1]).Path.Path);
			Assert.Equal(false, binding.Converter.Convert(new object[] {true, false}, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(new object[] {true, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, false}, null, null, null));
		}

		[Fact]
		public void OneWayWithFunction()
		{
			var singlePath = ExpressionToBindingParser.OneWay((Item item) => InstanceMethod(item.IsChecked));
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(false, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void ContextPropertyAndInstanceField()
		{
			var item1 = new Item(true);
			var item2 = new Item(false);
			var item3 = new Item(true);
			items = new HashSet<Item> {item1, item2};
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.IsChecked && items.Contains(x));
			var binding = (MultiBinding) expressionBinding.ToBindingBase();
			Assert.True(binding.Bindings.OfType<Binding>().Any<Binding>(childBinding => childBinding.Path.Path == "IsChecked"));
			Assert.True(binding.Bindings.OfType<Binding>().Any<Binding>(childBinding => childBinding.Path.Path == ""));
			Assert.Equal(true, expressionBinding.Evaluate(item1));
			Assert.Equal(false, expressionBinding.Evaluate(item2));
			Assert.Equal(false, expressionBinding.Evaluate(item3));
		}

		[Fact]
		public void InstancePath()
		{
			var textBox = new TextBox();
			ExpressionToBindingParser.TwoWay(() => SomeText).Apply(textBox, TextBox.TextProperty);
			var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
			Assert.Equal("SomeText", bindingExpression.ParentBinding.Path.Path);
			Assert.Equal(this, textBox.DataContext);
		}

		bool InstanceMethod(bool input)
		{
			return !input;
		}
	}
}