using System.CommandLine.Invocation;
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
        private static InputContract NullPropertySetter<InputContract>(InputContract input, InvocationContext invocationContext) => input;

        private static Symbol? GetSymbolForCurrentCommand(InvocationContext context, string name)
        {
            var executedCommand = context.ParseResult.CommandResult.Symbol;
            Symbol? matchingSymbolResult = executedCommand.Children.FirstOrDefault(symbol => IsAliasMatch(symbol, name));
            return matchingSymbolResult;
        }

        public static IPropertyBinder<InputContract> FromName<InputContract, TProperty>(string name, Func<InputContract, TProperty, InputContract> setter)
        {
            return PropertyBinder.FromFunc((InputContract inputContract, InvocationContext context) =>
            {
                IPropertyBinder<InputContract> mapFn = GetSymbolForCurrentCommand(context, name) switch
                {
                    null => throw new ArgumentException($"No input symbol for alias {name}"),
                    Argument<TProperty> argRef => FromReference(argRef, setter),
                    Option<TProperty> argRef => FromReference(argRef, setter),
                    _ => throw new ArgumentException($"Symbol with {name} is not an Option<{typeof(TProperty)} or Arugument<{typeof(TProperty)}>")
                };
                return mapFn.Bind(inputContract, context);
            });
        }

        public static IPropertyBinder<InputContract> FromReference<InputContract, TProperty>(Option<TProperty> optionRef, Func<InputContract, TProperty, InputContract> setter)
        {
            return PropertyBinder.FromFunc((InputContract inputContract, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForOption(optionRef);
                return setter(inputContract, propertyValue);
            });
        }
        public static IPropertyBinder<InputContract> FromReference<InputContract, TProperty>(Argument<TProperty> argumentRef, Func<InputContract, TProperty, InputContract> setter)
        {
            return PropertyBinder.FromFunc((InputContract inputContract, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForArgument(argumentRef);
                return setter(inputContract, propertyValue);
            });
        }

        public static IPropertyBinder<InputContract> FromReference<InputContract, TProperty>(Argument<TProperty> argumentRef, Expression<Func<InputContract, TProperty>> selectorLambda)
        {

            return FromReference<InputContract, TProperty>(argumentRef, (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            });
        }

        public static IPropertyBinder<InputContract> FromReference<InputContract, TProperty>(Option<TProperty> optionRef, Expression<Func<InputContract, TProperty>> selectorLambda)
        {
            return FromReference<InputContract, TProperty>(optionRef, (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            });
        }

        public static IPropertyBinder<InputContract> FromName<InputContract, TProperty>(string name, Expression<Func<InputContract, TProperty>> selectorLambda)
        {
            return FromName<InputContract, TProperty>(name, (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            });
        }
    }
}
