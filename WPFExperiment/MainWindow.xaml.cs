using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using WPFBindingGeneration;
using WPFBindingGeneration.ExpressionBindings;

namespace WPFExperiment
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		readonly ISet<Item> checkedItems = new HashSet<Item>();

		public MainWindow()
		{
			InitializeComponent();
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
			AddDependsOnInstance();
			SuperGrid.ItemsSource = items;
		}

		void AddDependsOnInstance()
		{
			AddColumn(ExpressionToBindingParser.OneWay((Item x) => x.IsChecked && checkedItems.Contains(x)).ToBindingBase(), "Depends on instance");
		}

		void AddConstantColumns()
		{
			AddColumn(ExpressionBindings.Convert((Item x) => false).ToBindingBase(), "Constant false");
			AddColumn(ExpressionBindings.Convert((Item x) => true).ToBindingBase(), "Constant true");
		}

		void AddIsNotCheckedOneWayColumn()
		{
			AddColumn(ExpressionBindings.Path((Item item) => item.IsChecked).Convert(b => !b).ToBindingBase(), "Not checked read-only");
			AddColumn(ExpressionToBindingParser.OneWay((Item item) => !item.IsChecked).ToBindingBase(), "Not checked read-only2");
		}

		void AddIsNotCheckedTwoWayColumn()
		{
			var binding = ExpressionBindings.Path((Item item) => item.IsChecked).Convert(b => !b, b => !b).ToBindingBase();
			var header = "Not checked writable";
			AddColumn(binding, header);
		}

		void AddOneTimeAlwaysCheckedColumn()
		{
			var binding = ExpressionBindings.Root<Item>().Convert(IsNotChecked).ToBindingBase();
			AddColumn(binding, "Not checked (one time)");
		}

		void AddIsCheckedColumn()
		{
			var binding = ExpressionBindings.Path((Item item) => item.IsChecked).ToBindingBase();
			AddColumn(binding, "Is checked");
		}

		void AddChildIsCheckedTwoWayColumn()
		{
			var binding = ExpressionToBindingParser.TwoWay((Item item) => item.ChildItem.IsChecked).ToBindingBase();
			AddColumn(binding, "Is child checked");
		}

		void AddChildIsNotCheckedOneWayColumn()
		{
			var binding = ExpressionToBindingParser.OneWay((Item item) => !item.ChildItem.IsChecked).ToBindingBase();
			AddColumn(binding, "Is child not checked (read-only)");
		}

		void AddBothChildAndBindingAreChecked()
		{
			var binding = ExpressionToBindingParser.OneWay((Item item) => item.ChildItem.IsChecked && item.IsChecked).ToBindingBase();
			AddColumn(binding, "BothChildAndBindingAreChecked");
		}

		void AddColumn(BindingBase binding, string header)
		{
			var isCheckedColumn = new DataGridCheckBoxColumn();
			isCheckedColumn.Binding = binding;
			isCheckedColumn.Header = header;
			SuperGrid.Columns.Add(isCheckedColumn);
		}

		public bool IsNotChecked(Item item)
		{
			return !item.IsChecked;
		}
	}
}