using System;
using System.Linq.Expressions;
using System.Windows.Data;

namespace WPFExperiment.BindingGenerators
{
    class OldBindingGenerator {

        public static Binding OneTime<T, U>(Func<T, U> func)
        {
            var result = new Binding();
            result.Mode = BindingMode.OneTime;
            result.Converter = new DelegateConverter<T, U>(func, null);
            return result;
        }
        
        public static Binding OneWay<T, U>(Expression<Func<T, U>> expression)
        {
            return ExpressionToBinding<T, U>(expression.Body);
        }


        /// <summary>
        /// Loopt van buiten naar binnen. 
        /// AST mag geen vertakkingen hebben???
        /// 
        /// Pas vanaf een property kan je een path maken.
        /// 
        /// OneWay betekent dat het converter stuk makkelijk te genereren is, en je property slechts een getter hoeft te hebben.
        /// Bij OneWay mag je ook method calls gebruiken.
        /// 
        /// Wil je zoiets als x => x.Age + 3 kunnen parsen. TwoWay zelfs?
        /// x => x.Age + x.Age kan natuurlijk niet.
        /// 
        /// Je moet beide paden van de tree afgaan en aan beide vragen of ze een path opleveren.
        /// Mogelijkheden zijn nee, ik ben constant, of ja een path en een 1-arity-functie.
        /// Hier dus Nee(3) en Ja("Age",x=>x)
        /// Combineren dmv + levert Ja("Age",x => x + 3)
        /// Je kan bij combineren maar 1 Ja met meerdere Nee's combineren.
        /// 
        /// Dit lijkt me allemaal veelste fucking magic. TwoWay is het misschien beter om alleen paden te ondersteunen, toch?
        /// Misschien dat je een TwoWayMagic kan maken die wel kinky stuffz doet.
        /// </summary>
        private static Binding ExpressionToBinding<T, U>(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return OneTime(Expression.Lambda<Func<T,U>>(expression, Expression.Parameter(typeof(T))).Compile());
                case ExpressionType.Negate:
                    return OneTime(Expression.Lambda<Func<T, U>>(expression, Expression.Parameter(typeof(T))).Compile());
                case ExpressionType.Add:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            throw new NotImplementedException();
        }

        //case ExpressionType.AddChecked:
        //            break;
        //        case ExpressionType.And:
        //            break;
        //        case ExpressionType.AndAlso:
        //            break;
        //        case ExpressionType.ArrayLength:
        //            break;
        //        case ExpressionType.ArrayIndex:
        //            break;
        //        case ExpressionType.Call:
        //            break;
        //        case ExpressionType.Coalesce:
        //            break;
        //        case ExpressionType.Conditional:
        //            break;
        //        case ExpressionType.Convert:
        //            break;
        //        case ExpressionType.ConvertChecked:
        //            break;
        //        case ExpressionType.Divide:
        //            break;
        //        case ExpressionType.Equal:
        //            break;
        //        case ExpressionType.ExclusiveOr:
        //            break;
        //        case ExpressionType.GreaterThan:
        //            break;
        //        case ExpressionType.GreaterThanOrEqual:
        //            break;
        //        case ExpressionType.Invoke:
        //            break;
        //        case ExpressionType.Lambda:
        //            throw new NotSupportedException();
        //        case ExpressionType.LeftShift:
        //            break;
        //        case ExpressionType.LessThan:
        //            break;
        //        case ExpressionType.LessThanOrEqual:
        //            break;
        //        case ExpressionType.ListInit:
        //            break;
        //        case ExpressionType.MemberAccess:
        //            break;
        //        case ExpressionType.MemberInit:
        //            break;
        //        case ExpressionType.Modulo:
        //            break;
        //        case ExpressionType.Multiply:
        //            break;
        //        case ExpressionType.MultiplyChecked:
        //            break;
        //        case ExpressionType.UnaryPlus:
        //            break;
        //        case ExpressionType.NegateChecked:
        //            break;
        //        case ExpressionType.New:
        //            break;
        //        case ExpressionType.NewArrayInit:
        //            break;
        //        case ExpressionType.NewArrayBounds:
        //            break;
        //        case ExpressionType.Not:
        //            break;
        //        case ExpressionType.NotEqual:
        //            break;
        //        case ExpressionType.Or:
        //            break;
        //        case ExpressionType.OrElse:
        //            break;
        //        case ExpressionType.Parameter:
        //            break;
        //        case ExpressionType.Power:
        //            break;
        //        case ExpressionType.Quote:
        //            break;
        //        case ExpressionType.RightShift:
        //            break;
        //        case ExpressionType.Subtract:
        //            break;
        //        case ExpressionType.SubtractChecked:
        //            break;
        //        case ExpressionType.TypeAs:
        //            break;
        //        case ExpressionType.TypeIs:
        //            break;
        //        case ExpressionType.Assign:
        //            break;
        //        case ExpressionType.Block:
        //            break;
        //        case ExpressionType.DebugInfo:
        //            break;
        //        case ExpressionType.Decrement:
        //            break;
        //        case ExpressionType.Dynamic:
        //            break;
        //        case ExpressionType.Default:
        //            break;
        //        case ExpressionType.Extension:
        //            break;
        //        case ExpressionType.Goto:
        //            break;
        //        case ExpressionType.Increment:
        //            break;
        //        case ExpressionType.Index:
        //            break;
        //        case ExpressionType.Label:
        //            break;
        //        case ExpressionType.RuntimeVariables:
        //            break;
        //        case ExpressionType.Loop:
        //            break;
        //        case ExpressionType.Switch:
        //            break;
        //        case ExpressionType.Throw:
        //            break;
        //        case ExpressionType.Try:
        //            break;
        //        case ExpressionType.Unbox:
        //            break;
        //        case ExpressionType.AddAssign:
        //            break;
        //        case ExpressionType.AndAssign:
        //            break;
        //        case ExpressionType.DivideAssign:
        //            break;
        //        case ExpressionType.ExclusiveOrAssign:
        //            break;
        //        case ExpressionType.LeftShiftAssign:
        //            break;
        //        case ExpressionType.ModuloAssign:
        //            break;
        //        case ExpressionType.MultiplyAssign:
        //            break;
        //        case ExpressionType.OrAssign:
        //            break;
        //        case ExpressionType.PowerAssign:
        //            break;
        //        case ExpressionType.RightShiftAssign:
        //            break;
        //        case ExpressionType.SubtractAssign:
        //            break;
        //        case ExpressionType.AddAssignChecked:
        //            break;
        //        case ExpressionType.MultiplyAssignChecked:
        //            break;
        //        case ExpressionType.SubtractAssignChecked:
        //            break;
        //        case ExpressionType.PreIncrementAssign:
        //            break;
        //        case ExpressionType.PreDecrementAssign:
        //            break;
        //        case ExpressionType.PostIncrementAssign:
        //            break;
        //        case ExpressionType.PostDecrementAssign:
        //            break;
        //        case ExpressionType.TypeEqual:
        //            break;
        //        case ExpressionType.OnesComplement:
        //            break;
        //        case ExpressionType.IsTrue:
        //            break;
        //        case ExpressionType.IsFalse:
        //            break;
    }
}