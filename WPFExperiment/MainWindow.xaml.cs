using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFExperiment.BindingGenerators;

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
            AddIsCheckedColumn();
            AddConstantColumns();
            AddOneTimeAlwaysCheckedColumn();
            AddIsNotCheckedOneWayColumn();
            AddIsNotCheckedTwoWayColumn();
            SuperGrid.ItemsSource = items;
        }

        private void AddConstantColumns()
        {
            var alwaysFalseColumn = new DataGridCheckBoxColumn();
            alwaysFalseColumn.Binding = BindingGenerator.Convert((Item x) => false).ToBinding();
            alwaysFalseColumn.Header = "Constant false";
            SuperGrid.Columns.Add(alwaysFalseColumn);
            
            var alwaysTrueColumn = new DataGridCheckBoxColumn();
            alwaysTrueColumn.Binding = BindingGenerator.Convert((Item x) => true).ToBinding();
            alwaysTrueColumn.Header = "Constant true";
            SuperGrid.Columns.Add(alwaysTrueColumn);
        }

        private void AddIsNotCheckedOneWayColumn()
        {
            var isCheckedColumn = new DataGridCheckBoxColumn();
            isCheckedColumn.Binding = BindingGenerator.Path((Item item) => item.IsChecked).Convert(b => !b).ToBinding();
            isCheckedColumn.Header = "Not checked read-only";
            SuperGrid.Columns.Add(isCheckedColumn);
        }

        private void AddIsNotCheckedTwoWayColumn()
        {
            var isCheckedColumn = new DataGridCheckBoxColumn();
            var binding = BindingGenerator.Path((Item item) => item.IsChecked).Convert(b => !b, b => !b).ToBinding();
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            isCheckedColumn.Binding = binding;
            isCheckedColumn.Header = "Not checked writable";
            SuperGrid.Columns.Add(isCheckedColumn);
        }

        private void AddOneTimeAlwaysCheckedColumn()
        {
            var isCheckedColumn = new DataGridCheckBoxColumn();
            isCheckedColumn.Binding = BindingGenerator.Root<Item>().Convert(IsNotChecked).ToBinding();
            isCheckedColumn.Header = "Not checked (one time)";
            SuperGrid.Columns.Add(isCheckedColumn);
        }

        private void AddIsCheckedColumn()
        {
            var isCheckedColumn = new DataGridCheckBoxColumn();
            var binding = BindingGenerator.Path((Item item) => item.IsChecked).ToBinding();
            isCheckedColumn.Binding = binding;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            isCheckedColumn.Header = "Is checked";
            SuperGrid.Columns.Add(isCheckedColumn);
        }

        public bool IsNotChecked(Item item)
        {
            return !item.IsChecked;
        }
    }
}
