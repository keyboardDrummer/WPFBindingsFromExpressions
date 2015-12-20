# WPFBindingsFromExpressions
Allows you to write WPF bindings using C# expression trees.

Both OneWay and TwoWay bindings can be generated, depending on what's possible given the original expression tree:

    ExpressionToBindingParser.TwoWay(() => Child.InputText).Apply(TextBox, TextBox.TextProperty);
    ExpressionToBindingParser.OneWay(() => "You wrote: " + Child.InputText).Apply(Label, ContentProperty);

By using an expression tree that takes an argument, bindings that use a context value are created:

    var isCheckedColumn = new DataGridCheckBoxColumn();
    isCheckedColumn.Binding = ExpressionToBindingParser.OneWay((Item item) => 
      item.ChildItem.IsChecked && item.IsChecked).ToBindingBase();
	  
##### Details
The expression tree is parsed by the framework and converted into a WPF BindingBase. 
From the expression tree, as much as possible is converted into WPF paths, 
while the remaining part of the expression tree is moved into a WPF value converter. 
By putting the expression tree into WPF paths, 
WPF will track these paths using INotifyPropertyChange and update the bound controls when required.
