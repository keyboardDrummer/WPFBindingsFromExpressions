using System.Reflection;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
    class PropertyAccess : IPathElement
    {
        private readonly PropertyInfo property;

        public PropertyAccess(PropertyInfo property)
        {
            this.property = property;
        }

        public PropertyInfo Property
        {
            get { return property; }
        }

        public override string ToString()
        {
            return property.Name;
        }

        public bool Writable {
            get { return property.CanWrite; }
        }
    }
}