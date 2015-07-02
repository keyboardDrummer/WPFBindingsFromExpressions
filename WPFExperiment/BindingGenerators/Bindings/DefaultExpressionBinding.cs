using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
	public abstract class DefaultExpressionBinding : IExpressionBinding
	{
		public abstract BindingBase ToBindingBase();
		public abstract bool IsWritable { get; }
	}
}