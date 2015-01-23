using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WPFExperiment.BindingGenerators
{
    internal static class BindingGenerator
    {
        public static IBindingSpec<From,To> Path<From, To>(Expression<Func<From, To>> func)
        {
            return new PathBinding<From, To>(func);
        }

        public static IBindingSpec<From, To> Convert<From, To>(Func<From, To> func)
        {
            return Root<From>().Convert(func);
        } 

        public static IBindingSpec<From,From> Root<From>()
        {
            return Path((From x) => x);
        }

    }
}
