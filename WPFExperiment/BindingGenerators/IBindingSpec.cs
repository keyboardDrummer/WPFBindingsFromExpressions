using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
    interface IBindingSpec<From, To>
    {
        Binding ToBinding();
        bool IsWritable { get; }
    }

    abstract class DefaultBindingSpec<From, To> : IBindingSpec<From, To>
    {
        public abstract Binding ToBindingBase();

        public Binding ToBinding()
        {
            var result = ToBindingBase();
            result.Mode = IsWritable ? BindingMode.TwoWay : BindingMode.OneWay;
            return result;
        }

        public abstract bool IsWritable { get; }
    }
}
