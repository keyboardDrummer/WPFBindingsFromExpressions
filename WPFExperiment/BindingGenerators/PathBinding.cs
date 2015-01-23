using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
    class PathBinding<From, To> : DefaultBindingSpec<From, To>
    {
        private IList<IPathElement> Elements;

        public PathBinding(IList<IPathElement> elements)
        {
            Elements = elements;
        }

        public override Binding ToBindingBase()
        {
            return new Binding(String.Join(".", Elements.Select(element => element.ToString()).Where(s => !string.IsNullOrEmpty(s))));
        }

        public override bool IsWritable
        {
            get { return Elements.Last().Writable; }
        }

        public ConverterBinding<From, To, V> Convert<V>(Func<To, V> forward, Func<V, To> backward = null)
        {
            return new ConverterBinding<From, To, V>(this,forward,backward);
        }
    }
}