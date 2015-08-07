using System.Windows.Data;
using WPFBindingGeneration;
using Xunit;

namespace WPFExperimentTests.BindingGenerators
{
	public class ExpressionParserTests
	{
		static readonly Item staticItem = new Item(false);

		[Fact]
		public void MultiBindingForReusedPath()
		{
			var binding = ExpressionToBindingParser.OneWay((Item i) => i.IsChecked && i.IsChecked);
			Assert.True(binding.ToBindingBase() is Binding);
		}

		[Fact]
		public void MultiBindingForReusedStaticPath()
		{
			var binding = ExpressionToBindingParser.OneWay(() => staticItem.IsChecked && staticItem.IsChecked);
			Assert.True(binding.ToBindingBase() is Binding);
		}

		[Fact]
		public void MultiBindingForReusedStaticPath2()
		{
			var binding = ExpressionToBindingParser.OneWay(() => staticItem == null ? "Default" : staticItem.ToString());
			Assert.True(binding.ToBindingBase() is Binding);
		}
	}
}