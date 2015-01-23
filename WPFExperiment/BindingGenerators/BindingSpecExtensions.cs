using System;

namespace WPFExperiment.BindingGenerators
{
    static class BindingSpecExtensions
    {
        public static IBindingSpec<From, To> Convert<From, U, To>(this IBindingSpec<From, U> spec, Func<U, To> forward,
            Func<To, U> backward = null)
        {
            return new ConverterBinding<From, U, To>((PathBinding<From, U>) spec, forward, backward);
        } 
    }
}