using System;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
    class ConverterBinding<From, U, To> : DefaultBindingSpec<From, To>
    {
        private readonly PathBinding<From, U> path;
        private readonly Func<U, To> forward;
        private readonly Func<To, U> backward;

        public ConverterBinding(PathBinding<From, U> path, Func<U, To> forward, Func<To, U> backward)
        {
            this.path = path;
            this.forward = forward;
            this.backward = backward;
        }

        public override Binding ToBindingBase()
        {
            var result = path.ToBindingBase();
            result.Converter = new DelegateConverter<U, To>(forward, backward);
            return result;
        }

        public override bool IsWritable { get { return path.IsWritable && backward != null; } }
    }
}