using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.CommandLine.PropertyMapBinder
{
    /// <summary>
    /// Cherry-picked from AutoMapper: https://github.com/AutoMapper/AutoMapper/blob/bdc0120497d192a2741183415543f6119f50a982/src/AutoMapper/Internal/ReflectionHelper.cs
    /// </summary>
    internal static class ReflectionHelper
    {

        private static ArgumentOutOfRangeException Expected(MemberInfo propertyOrField) => new ArgumentOutOfRangeException(nameof(propertyOrField), "Expected a property or field, not " + propertyOrField);

        public static void SetMemberValue(this MemberInfo propertyOrField, object target, object value)
        {
            if (propertyOrField is PropertyInfo property)
            {
                property.SetValue(target, value, null);
                return;
            }
            if (propertyOrField is FieldInfo field)
            {
                field.SetValue(target, value);
                return;
            }
            throw Expected(propertyOrField);
        }

        public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression.Body;
            while (true)
            {
                switch (expressionToCheck)
                {
                    case MemberExpression { Member: var member, Expression: { NodeType: ExpressionType.Parameter or ExpressionType.Convert } }:
                        return member;
                    case UnaryExpression { Operand: var operand }:
                        expressionToCheck = operand;
                        break;
                    default:
                        throw new ArgumentException(
                            $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
                            nameof(lambdaExpression));
                }
            }
        }
    }
}
