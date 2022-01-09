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
    /// Cherry-picked from AutoMapper: https://github.com/AutoMapper/AutoMapper/blob/v10.0.0/src/AutoMapper/Internal/ReflectionHelper.cs
    /// - cleaner versions exist for netstandard2.1+ https://github.com/AutoMapper/AutoMapper/blob/v11.0.0/src/AutoMapper/Internal/ReflectionHelper.cs
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
            Expression expressionToCheck = lambdaExpression;

            var done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = ((LambdaExpression)expressionToCheck).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = ((MemberExpression)expressionToCheck);

                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException(
                                $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
                                nameof(lambdaExpression));
                        }

                        var member = memberExpression.Member;

                        return member;
                    default:
                        done = true;
                        break;
                }
            }

            throw new InvalidOperationException("Couldn't resolve a member from given selector.");
        }
    }
}
