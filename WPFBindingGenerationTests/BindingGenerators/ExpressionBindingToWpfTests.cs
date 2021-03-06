﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFBindingGeneration;
using WPFBindingGeneration.ExpressionBindings;
using Xunit;

namespace WPFExperimentTests.BindingGenerators
{
	public class ExpressionBindingToWpfTests
	{
		static readonly Item staticItem = new Item(false);
		static readonly Item staticItemWithChild = CreateItemWithChild(false, true);
		ISet<Item> items;

		public ExpressionBindingToWpfTests()
		{
			var subItem = new SubItem(true);
			ActuallySubItem = subItem;
			subItem.AnotherProperty = 3;
		}

		public string SomeText { get; set; }
		public bool IsChecked { get; set; }

		public static Item StaticItemProperty
		{
			get { return staticItem; }
		}

		public bool ProtectedSetBool { get; protected set; }

		public Item ActuallySubItem { get; set; }

		[Fact]
		public void CastDisappearsInpath()
		{
			var staticProperty = ExpressionToBindingParser.OneWay(() => ((SubItem) ActuallySubItem).AnotherProperty);
			var binding = (Binding) staticProperty.ToBindingBase();
			Assert.Equal("ActuallySubItem.AnotherProperty", binding.Path.Path);
		}

		[Fact]
		public void ParseBinaryType()
		{
			// ReSharper disable once IsExpressionAlwaysTrue
			var staticProperty = ExpressionToBindingParser.OneWay(() => staticItem is Item);
			Assert.True(staticProperty.Evaluate(Unit.Instance));

			// ReSharper disable once IsExpressionAlwaysTrue
			var staticProperty2 = ExpressionToBindingParser.OneWay(() => staticItem is object);
			Assert.True(staticProperty2.Evaluate(Unit.Instance));
		}

		[Fact]
		public void PathlessStaticField()
		{
			var staticProperty = ExpressionToBindingParser.OneWay(() => staticItem);
			var binding = (Binding) staticProperty.ToBindingBase();
			Assert.Equal(null, binding.Path);
		}

		[Fact]
		public void VirtualGetOnlyProperty()
		{
			var containsMustAccept = new ContainsMustAccept();
			// ReSharper disable once SimplifyConditionalTernaryExpression
			var staticProperty = ExpressionToBindingParser.OneWay(() => BoolToVisibility(containsMustAccept == null ? false : containsMustAccept.MustAccept));
			var binding = staticProperty.ToBindingBase();
			var textBox = new TextBox();
			Assert.Equal(Visibility.Visible, textBox.Visibility);
			textBox.SetBinding(UIElement.VisibilityProperty, binding);
			textBox.BeginInit();
			textBox.EndInit();
			Assert.Equal(Visibility.Collapsed, textBox.Visibility);
			//var bindingExpression = textBox.GetBindingExpression(UIElement.VisibilityProperty);
			//Assert.Equal(BindingStatus.Active, bindingExpression.Status);
		}

		static Visibility BoolToVisibility(bool visible)
		{
			return !visible ? Visibility.Visible : Visibility.Collapsed;
		}

		[Fact]
		public void ProtectedSetMeansOneWay()
		{
			var staticProperty = ExpressionToBindingParser.OneWay(() => ProtectedSetBool);
			var binding = (Binding) staticProperty.ToBindingBase();
			Assert.Equal(BindingMode.OneWay, binding.Mode);
		}

		static Item CreateItemWithChild(bool isChecked, bool isChildCheck)
		{
			return new Item(isChecked)
			{
				ChildItem = new Item(isChildCheck)
			};
		}

		[Fact]
		public void StaticProperty()
		{
			var staticProperty = ExpressionToBindingParser.TwoWay(() => StaticItemProperty.IsChecked);
			Assert.Equal(false, staticProperty.Evaluate(Unit.Instance));
			var binding = (Binding) staticProperty.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
		}

		[Fact]
		public void EvaluateNullPath()
		{
			var root = new Item(false);

			var paramChildIsChecked = ExpressionToBindingParser.TwoWay((Item item) => item.ChildItem.IsChecked);
			Assert.Equal(false, paramChildIsChecked.Evaluate(root));

			var paramChildChild = ExpressionToBindingParser.TwoWay((Item item) => item.ChildItem.ChildItem);
			Assert.Equal(null, paramChildChild.Evaluate(root));
		}

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
			Assert.Equal("IsChecked", ((Binding) binding.Bindings[1]).Path.Path);
			Assert.Equal("ChildItem.IsChecked", ((Binding) binding.Bindings[0]).Path.Path);
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
			Assert.True(binding.Bindings.OfType<Binding>().Any(childBinding => childBinding.Path.Path == "IsChecked"));
			Assert.True(binding.Bindings.OfType<Binding>().Any(childBinding => childBinding.Path.Path == ""));
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
			Assert.Equal(this, textBox.GetBindingExpression(TextBox.TextProperty).ParentBinding.Source);
		}

		bool InstanceMethod(bool input)
		{
			return !input;
		}

		[Fact]
		public void ContextPathAndInstancePath()
		{
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.IsChecked && IsChecked);
			var binding = (MultiBinding) expressionBinding.ToBindingBase();
			var bindings = binding.Bindings.OfType<Binding>().ToList();
			Assert.True(bindings.Any(childBinding => childBinding.Source == this && childBinding.Path.Path == "IsChecked"));
			Assert.True(bindings.Any(childBinding => childBinding.Source == null && childBinding.Path.Path == "IsChecked"));
		}

		[Fact]
		public void MemberCallWithPathInstance()
		{
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.ChildItem.SomeMethod(true).IsChecked);
			var binding = (Binding) expressionBinding.ToBindingBase();
			Assert.Equal("ChildItem", binding.Path.Path);
			var root = new Item(true);
			root.ChildItem = new Item(false);
			Assert.Equal(true, expressionBinding.Evaluate(root));
		}

		[Fact]
		public void MemberCallMemberExpression()
		{
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.ChildItem.SomeMethod(x.IsChecked).IsChecked);
			var binding = (MultiBinding) expressionBinding.ToBindingBase();
			Assert.True(binding.Bindings.OfType<Binding>().Any(childBinding => childBinding.Path.Path == "ChildItem"));
			Assert.True(binding.Bindings.OfType<Binding>().Any(childBinding => childBinding.Path.Path == "IsChecked"));
			var root = new Item(true);
			root.ChildItem = new Item(false);
			Assert.Equal(true, expressionBinding.Evaluate(root));

			root.IsChecked = false;
		}

		[Fact]
		public void TestNullConversion()
		{
			var binding = ExpressionToBindingParser.OneWay((Item x) => x.ChildItem == null ? "None" : x.ChildItem.ToString());
			Assert.Equal("None", binding.Evaluate(null));
		}

		[Fact]
		public void TestStaticMethod()
		{
			var binding = ExpressionToBindingParser.OneWay((Item x) => StaticMethod());
			Assert.Equal(true, binding.Evaluate(null));
		}

		[Fact]
		public void MultiplePathsBothOneStatic()
		{
			var singlePath = ExpressionToBindingParser.OneWay((Item item) => item.IsChecked && staticItemWithChild.ChildItem.IsChecked);
			var binding = (MultiBinding) singlePath.ToBindingBase();
			var bindings = binding.Bindings.OfType<Binding>().ToList();
			Assert.True(bindings.Any(childBinding => childBinding.Source == null && childBinding.Path.Path == "IsChecked"));
			Assert.True(bindings.Any(childBinding => childBinding.Source == staticItemWithChild && childBinding.Path.Path == "ChildItem.IsChecked"));
			Assert.Equal(true, binding.Converter.Convert(new object[] {true, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, true}, null, null, null));
		}

		static bool StaticMethod()
		{
			return true;
		}
	}
}