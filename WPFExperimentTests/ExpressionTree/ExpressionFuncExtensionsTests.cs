using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using WPFBindingGeneration;
using WPFBindingGeneration.ExpressionBindings;
using WPFBindingGeneration.ExpressionFunc;
using Xunit;

namespace WPFExperimentTests.ExpressionTree
{
	public class ExpressionFuncExtensionsTests
	{
		static readonly Item staticItem = new Item(false);

		static readonly Item staticItemWithChild = new Item(false)
		{
			ChildItem = new Item(true)
		};

		ISet<Item> items;
		public string SomeText { get; set; }
		public bool IsChecked { get; set; }

		Item CreateItemWithChild(bool isChecked, bool isChildCheck)
		{
			return new Item(isChecked)
			{
				ChildItem = new Item(isChildCheck)
			};
		}

		[Fact]
		public void ComposeContextualContextual()
		{
			var itemIsChecked = ExpressionFuncExtensions.Create((Item item) => item.IsChecked);
			var isChildIsChecked = ExpressionFuncExtensions.Create((Item item) => item.ChildItem.IsChecked);
			var andCombo = ExpressionFuncExtensions.Compose(itemIsChecked, isChildIsChecked, (a, b) => a && b);
			var binding = (MultiBinding) andCombo.ExpressionBinding.ToBindingBase();
			Assert.Equal(true, andCombo.Evaluate(CreateItemWithChild(true, true)));
			Assert.Equal(false, andCombo.Evaluate(CreateItemWithChild(true, false)));
			Assert.Equal(false, andCombo.Evaluate(CreateItemWithChild(false, true)));
			Assert.Equal(false, andCombo.Evaluate(CreateItemWithChild(false, false)));

			Assert.Equal("IsChecked", ((Binding) binding.Bindings[1]).Path.Path);
			Assert.Equal("ChildItem.IsChecked", ((Binding) binding.Bindings[0]).Path.Path);
			Assert.Equal(false, binding.Converter.Convert(new object[] {true, false}, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(new object[] {true, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, false}, null, null, null));
		}

		[Fact]
		public void ComposeContextualContextFree()
		{
			var itemIsChecked = ExpressionFuncExtensions.Create((Item item) => item.IsChecked);
			var isChildIsChecked = ExpressionFuncExtensions.Create(() => staticItem.ChildItem.IsChecked);
			var andCombo = ExpressionFuncExtensions.Compose(itemIsChecked, isChildIsChecked, (a, b) => a && b);

			var binding = (Binding) andCombo.ExpressionBinding.ToBindingBase();

			Assert.Equal(true, andCombo.Evaluate(new Item(true)));
			Assert.Equal(false, andCombo.Evaluate(new Item(false)));

			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(true, binding.Converter.Convert(new object[] {true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false}, null, null, null));
		}

		[Fact]
		public void EvaluateNullPath()
		{
			var root = new Item(false);

			var paramChildIsChecked = ExpressionFuncExtensions.Create((Item item) => item.ChildItem.IsChecked);
			Assert.Equal(false, paramChildIsChecked.Evaluate(root));

			var paramChildChild = ExpressionToBindingParser.TwoWay((Item item) => item.ChildItem.ChildItem);
			Assert.Equal(null, paramChildChild.Evaluate(root));
		}

		[Fact]
		public void SinglePathNoConverter()
		{
			var singlePath = ExpressionFuncExtensions.Create((Item item) => item.IsChecked);
			var binding = (Binding) singlePath.ExpressionBinding.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
		}

		[Fact]
		public void SinglePathWithNotConverter()
		{
			var singlePath = ExpressionFuncExtensions.Create((Item item) => !item.IsChecked);
			var binding = (Binding) singlePath.ExpressionBinding.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(false, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void SinglePathWithBinaryConverter()
		{
			var singlePath = ExpressionFuncExtensions.Create((Item item) => item.IsChecked || true);
			var binding = (Binding) singlePath.ExpressionBinding.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(true, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void OneWayWithFunction()
		{
			var singlePath = ExpressionFuncExtensions.Create((Item item) => InstanceMethod(item.IsChecked));
			var binding = (Binding) singlePath.ExpressionBinding.ToBindingBase();
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
			ExpressionFuncExtensions.Create(() => SomeText).ExpressionBinding.Apply(textBox, TextBox.TextProperty);
			var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
			Assert.Equal("SomeText", bindingExpression.ParentBinding.Path.Path);
			Assert.Equal(this, textBox.DataContext);
		}

		bool InstanceMethod(bool input)
		{
			return !input;
		}

		[Fact]
		public void ContextPathAndInstancePath()
		{
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.IsChecked && IsChecked);
			Assert.Throws<ArgumentException>(() => expressionBinding.Check());
		}

		[Fact]
		public void MemberCallWithPathInstance()
		{
			var expressionBinding = ExpressionFuncExtensions.Create((Item x) => x.ChildItem.SomeMethod(true).IsChecked);
			var binding = (Binding) expressionBinding.ExpressionBinding.ToBindingBase();
			Assert.Equal("ChildItem", binding.Path.Path);
			var root = new Item(true);
			root.ChildItem = new Item(false);
			Assert.Equal(true, expressionBinding.Evaluate(root));
		}

		[Fact]
		public void MemberCallMemberExpression()
		{
			var expressionBinding = ExpressionFuncExtensions.Create((Item x) => x.ChildItem.SomeMethod(x.IsChecked).IsChecked);
			var binding = (MultiBinding) expressionBinding.ExpressionBinding.ToBindingBase();
			Assert.True(binding.Bindings.OfType<Binding>().Any<Binding>(childBinding => childBinding.Path.Path == "IsChecked"));
			Assert.True(binding.Bindings.OfType<Binding>().Any<Binding>(childBinding => childBinding.Path.Path == "ChildItem"));
			var root = new Item(true);
			root.ChildItem = new Item(false);
			Assert.Equal(true, expressionBinding.Evaluate(root));

			root.IsChecked = false;
		}

		[Fact]
		public void TestNullConversion()
		{
			var binding = ExpressionFuncExtensions.Create((Item x) => x.ChildItem == null ? "None" : x.ChildItem.ToString());
			Assert.Equal("None", binding.Evaluate(new Item(false)));
		}

		[Fact]
		public void TestStaticMethod()
		{
			var binding = ExpressionFuncExtensions.Create((Item x) => StaticMethod());
			Assert.Equal(true, binding.Evaluate(null));
		}

		static bool StaticMethod()
		{
			return true;
		}
	}
}