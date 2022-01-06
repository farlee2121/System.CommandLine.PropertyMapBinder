﻿using System.CommandLine.Invocation;
using System.Linq.Expressions;
using System.Reflection;

namespace System.CommandLine.PropertyMapBinder
{
    public static class PropertyMap
    {
        private static bool IsAliasMatch(ISymbol symbol, string alias)
        {
            //NOTE: Arguments are not an IdentifierSymbol and just Symbol doesn't have aliases
            return symbol switch
            {
                Option option => option.HasAlias(alias),
                Argument arg => arg.Name == alias,
                _ => false
            };

        }
        private static InputModel NullPropertySetter<InputModel>(InputModel input, InvocationContext invocationContext) => input;

        private static Symbol? GetSymbolForCurrentCommand(InvocationContext context, string name)
        {
            var executedCommand = context.ParseResult.CommandResult.Symbol;
            Symbol? matchingSymbolResult = executedCommand.Children.FirstOrDefault(symbol => IsAliasMatch(symbol, name));
            return matchingSymbolResult;
        }

        public static IPropertyBinder<InputModel> FromName<InputModel, TProperty>(string name, Func<InputModel, TProperty, InputModel> setter)
        {
            return PropertyBinder.FromFunc((InputModel InputModel, InvocationContext context) =>
            {
                IPropertyBinder<InputModel> mapFn = GetSymbolForCurrentCommand(context, name) switch
                {
                    null => throw new ArgumentException($"No input symbol for alias {name}"),
                    Argument<TProperty> argRef => FromReference(argRef, setter),
                    Option<TProperty> argRef => FromReference(argRef, setter),
                    _ => throw new ArgumentException($"Symbol with {name} is not an Option<{typeof(TProperty)} or Arugument<{typeof(TProperty)}>")
                };
                return mapFn.Bind(InputModel, context);
            });
        }
        public static IPropertyBinder<InputModel> FromName<InputModel, TProperty>(string name, Expression<Func<InputModel, TProperty>> selectorLambda)
        {
            return FromName<InputModel, TProperty>(name, (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            });
        }

        public static IPropertyBinder<InputModel> FromReference<InputModel, TProperty>(Option<TProperty> optionRef, Func<InputModel, TProperty, InputModel> setter)
        {
            return PropertyBinder.FromFunc((InputModel InputModel, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForOption(optionRef);
                return setter(InputModel, propertyValue);
            });
        }
        public static IPropertyBinder<InputModel> FromReference<InputModel, TProperty>(Option<TProperty> optionRef, Expression<Func<InputModel, TProperty>> selectorLambda)
        {
            return FromReference<InputModel, TProperty>(optionRef, (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            });
        }

        public static IPropertyBinder<InputModel> FromReference<InputModel, TProperty>(Argument<TProperty> argumentRef, Func<InputModel, TProperty, InputModel> setter)
        {
            return PropertyBinder.FromFunc((InputModel InputModel, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForArgument(argumentRef);
                return setter(InputModel, propertyValue);
            });
        }

        public static IPropertyBinder<InputModel> FromReference<InputModel, TProperty>(Argument<TProperty> argumentRef, Expression<Func<InputModel, TProperty>> selectorLambda)
        {

            return FromReference<InputModel, TProperty>(argumentRef, (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            });
        }

        

        
    }
}
