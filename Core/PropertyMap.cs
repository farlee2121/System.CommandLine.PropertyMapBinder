using System.CommandLine.Invocation;


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

        public static Func<InputContract, InvocationContext, InputContract> FromName<InputContract, TProperty>(string name, Func<InputContract, TProperty, InputContract> setter)
        {
            return (InputContract inputContract, InvocationContext context) =>
            {
                // how are aliases handled?
                var executedCommand = context.ParseResult.CommandResult.Symbol;
                Symbol? matchingSymbolResult = executedCommand.Children.FirstOrDefault(symbol => IsAliasMatch(symbol, name));

                Func<InputContract, InvocationContext, InputContract> mapFn = matchingSymbolResult switch
                {
                    null => throw new ArgumentException($"No input symbol for alias {name}"),
                    Argument<TProperty> argRef => FromReference(argRef, setter),
                    Option<TProperty> argRef => FromReference(argRef, setter),
                    _ => throw new ArgumentException($"Symbol with {name} is not an Option<{typeof(TProperty)} or Arugument<{typeof(TProperty)}>")
                };
                return mapFn(inputContract, context);
            };
        }

        public static Func<InputContract, InvocationContext, InputContract> FromReference<InputContract, TProperty>(Option<TProperty> optionRef, Func<InputContract, TProperty, InputContract> setter)
        {
            return (InputContract inputContract, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForOption<TProperty>(optionRef);
                return setter(inputContract, propertyValue);
            };
        }
        public static Func<InputContract, InvocationContext, InputContract> FromReference<InputContract, TProperty>(Argument<TProperty> argumentRef, Func<InputContract, TProperty, InputContract> setter)
        {
            return (InputContract inputContract, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForArgument<TProperty>(argumentRef);
                return setter(inputContract, propertyValue);
            };
        }
    }
}
