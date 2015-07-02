using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using WPFBindingGeneration;

namespace WPFExperiment
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			var firstItem = new Item(true);
			var items = new List<Item> {firstItem, new Item(false), new Item(true)};
			firstItem.ChildItem = new Item(true);
			items[1].ChildItem = new Item(true);
			items[2].ChildItem = new Item(true);
			AddIsCheckedColumn();
			AddConstantColumns();
			AddOneTimeAlwaysCheckedColumn();
			AddIsNotCheckedOneWayColumn();
			AddIsNotCheckedTwoWayColumn();
			AddChildIsCheckedTwoWayColumn();
			AddChildIsNotCheckedOneWayColumn();
			AddBothChildAndBindingAreChecked();
			SuperGrid.ItemsSource = items;
		}

		void AddConstantColumns()
		{
			AddColumn(BindingGenerator.Convert((Item x) => false).ToBindingBase(), "Constant false");
			AddColumn(BindingGenerator.Convert((Item x) => true).ToBindingBase(), "Constant true");
		}

		void AddIsNotCheckedOneWayColumn()
		{
			AddColumn(BindingGenerator.Path((Item item) => item.IsChecked).Convert(b => !b).ToBindingBase(), "Not checked read-only");
			AddColumn(BindingGenerator.OneWay((Item item) => !item.IsChecked).ToBindingBase(), "Not checked read-only2");
		}

		void AddIsNotCheckedTwoWayColumn()
		{
			var binding = BindingGenerator.Path((Item item) => item.IsChecked).Convert(b => !b, b => !b).ToBindingBase();
			var header = "Not checked writable";
			AddColumn(binding, header);
		}

		void AddOneTimeAlwaysCheckedColumn()
		{
			var binding = BindingGenerator.Root<Item>().Convert(IsNotChecked).ToBindingBase();
			AddColumn(binding, "Not checked (one time)");
		}

		void AddIsCheckedColumn()
		{
			var binding = BindingGenerator.Path((Item item) => item.IsChecked).ToBindingBase();
			AddColumn(binding, "Is checked");
		}

		void AddChildIsCheckedTwoWayColumn()
		{
			var binding = BindingGenerator.TwoWay((Item item) => item.ChildItem.IsChecked).ToBindingBase();
			AddColumn(binding, "Is child checked");
		}

		void AddChildIsNotCheckedOneWayColumn()
		{
			var binding = BindingGenerator.OneWay((Item item) => !item.ChildItem.IsChecked).ToBindingBase();
			AddColumn(binding, "Is child not checked (read-only)");
		}

		void AddBothChildAndBindingAreChecked()
		{
			var binding = BindingGenerator.OneWay((Item item) => item.ChildItem.IsChecked && item.IsChecked).ToBindingBase();
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