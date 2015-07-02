using System.Linq.Expressions;

namespace WPFBindingGeneration.ExpressionBindings.Paths
{
    class Parameter : IPathElement
    {
        private readonly ParameterExpression parameterExpression;

        public Parameter(ParameterExpression parameterExpression)
        {
            this.parameterExpression = parameterExpression;
        }

        public override string ToString()
        {
            return "";
        }

        public bool Writable { get { return false; }}
    }
}