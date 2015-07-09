using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WPFBindingGeneration;
using WPFBindingGeneration.ExpressionBindings;
using WPFExperiment.Properties;

namespace WPFExperiment
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : INotifyPropertyChanged
	{
		readonly ISet<Item> checkedItems = new HashSet<Item>();
		bool isChecked;
		string someText;

		public MainWindow()
		{
			InitializeComponent();
			AddGridExample();
			AddTextExample();
			ExpressionToBindingParser.TwoWay(() => IsChecked).Apply(CheckBox, ToggleButton.IsCheckedProperty);
		}

		public string SomeText
		{
			get { return someText; }
			set
			{
				someText = value;
				OnPropertyChanged();
			}
		}

		public bool IsChecked
		{
			get { return isChecked; }
			set
			{
				isChecked = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void AddTextExample()
		{
			SomeText = "jo";
			ExpressionToBindingParser.TwoWay(() => SomeText).Apply(SomeTextBox, TextBox.TextProperty);
			ExpressionToBindingParser.TwoWay(() => SomeText).Apply(SomeTextBox2, TextBox.TextProperty);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		void AddGridExample()
		{
			var firstItem = new Item(true);
			var items = new List<Item> {firstItem, new Item(false), new Item(true)};
			firstItem.ChildItem = new Item(true);
			items[1].ChildItem = new Item(true);
			checkedItems.Add(items[1]);
			items[2].ChildItem = new Item(true);
			checkedItems.Add(items[2]);
			AddIsCheckedColumn();
			AddConstantColumns();
			AddOneTimeAlwaysCheckedColumn();
			AddIsNotCheckedOneWayColumn();
			AddIsNotCheckedTwoWayColumn();
			AddChildIsCheckedTwoWayColumn();
			AddChildIsNotCheckedOneWayColumn();
			AddBothChildAndBindingAreChecked();
			AddDependsOnInstanceWithMethod();
			//AddDependsOnInstanceWithPath(); Doesn't work.
			SuperGrid.ItemsSource = items;
		}

		void AddDependsOnInstanceWithMethod()
		{
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.IsChecked && checkedItems.Contains(x));
			AddColumn(expressionBinding, "Depends on instance method");
		}

		void AddDependsOnInstanceWithPath()
		{
			var expressionBinding = ExpressionToBindingParser.OneWay((Item x) => x.IsChecked && IsChecked);
			expressionBinding.Check();
			AddColumn(expressionBinding, "Depends on instance path");
		}

		void AddConstantColumns()
		{
			AddColumn(ExpressionBindings.Convert((Item x) => false), "Constant false");
			AddColumn(ExpressionBindings.Convert((Item x) => true), "Constant true");
		}

		void AddIsNotCheckedOneWayColumn()
		{
			AddColumn(ExpressionBindings.Path((Item item) => item.IsChecked).Convert(b => !b), "Not checked read-only");
			AddColumn(ExpressionToBindingParser.OneWay((Item item) => !item.IsChecked), "Not checked read-only2");
		}

		void AddIsNotCheckedTwoWayColumn()
		{
			var binding = ExpressionBindings.Path((Item item) => item.IsChecked).Convert(b => !b, b => !b);
			var header = "Not checked writable";
			AddColumn(binding, header);
		}

		void AddOneTimeAlwaysCheckedColumn()
		{
			var binding = ExpressionBindings.Root<Item>().Convert(IsNotChecked);
			AddColumn(binding, "Not checked (one time)");
		}

		void AddIsCheckedColumn()
		{
			var binding = ExpressionBindings.Path((Item item) => item.IsChecked);
			AddColumn(binding, "Is checked");
		}

		void AddChildIsCheckedTwoWayColumn()
		{
			var binding = ExpressionToBindingParser.TwoWay((Item item) => item.ChildItem.IsChecked);
			AddColumn(binding, "Is child checked");
		}

		void AddChildIsNotCheckedOneWayColumn()
		{
			var binding = ExpressionToBindingParser.OneWay((Item item) => !item.ChildItem.IsChecked);
			AddColumn(binding, "Is child not checked (read-only)");
		}

		void AddBothChildAndBindingAreChecked()
		{
			var binding = ExpressionToBindingParser.OneWay((Item item) => item.ChildItem.IsChecked && item.IsChecked);
			AddColumn(binding, "BothChildAndBindingAreChecked");
		}

		void AddColumn(IExpressionBinding binding, string header)
		{
			var isCheckedColumn = new DataGridCheckBoxColumn();
			isCheckedColumn.Binding = binding.ToBindingBase();
			isCheckedColumn.Header = header;
			SuperGrid.Columns.Add(isCheckedColumn);
		}

		public bool IsNotChecked(Item item)
		{
			return !item.IsChecked;
		}
	}
}