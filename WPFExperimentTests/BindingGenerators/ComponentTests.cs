using System.Windows.Data;
using WPFBindingGeneration;
using Xunit;

namespace WPFExperimentTests.BindingGenerators
{
	public class ComponentTests
	{
		[Fact]
		public void SinglePathNoConverter()
		{
			var singlePath = BindingGenerator.TwoWay((Item item) => item.IsChecked);
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
		}

		[Fact]
		public void SinglePathWithNotConverter()
		{
			var singlePath = BindingGenerator.OneWay((Item item) => !item.IsChecked);
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(false, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void SinglePathWithBinaryConverter()
		{
			var singlePath = BindingGenerator.OneWay((Item item) => item.IsChecked || true);
			var binding = (Binding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", binding.Path.Path);
			Assert.Equal(true, binding.Converter.Convert(true, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(false, null, null, null));
		}

		[Fact]
		public void MultiplePathWithAnd()
		{
			var singlePath = BindingGenerator.OneWay((Item item) => item.IsChecked && item.ChildItem.IsChecked);
			var binding = (MultiBinding) singlePath.ToBindingBase();
			Assert.Equal("IsChecked", ((Binding) binding.Bindings[0]).Path.Path);
			Assert.Equal("ChildItem.IsChecked", ((Binding) binding.Bindings[1]).Path.Path);
			Assert.Equal(false, binding.Converter.Convert(new object[] {true, false}, null, null, null));
			Assert.Equal(true, binding.Converter.Convert(new object[] {true, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, true}, null, null, null));
			Assert.Equal(false, binding.Converter.Convert(new object[] {false, false}, null, null, null));
		}
	}
}