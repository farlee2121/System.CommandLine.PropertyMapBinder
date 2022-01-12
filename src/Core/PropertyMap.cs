using System.CommandLine.Invocation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.CommandLine.PropertyMapBinder
{
    public static partial class PropertyMap
    {
        private static bool IsAliasMatch(ISymbol symbol, string alias)
        {
            //NOTE: Arguments are not an IdentifierSymbol and just Symbol doesn't have aliases
            if (symbol is Option option) return option.HasAlias(alias);
            else if (symbol is Argument arg) return arg.Name == alias;
            else return false;

        }
        private static InputModel NullPropertySetter<InputModel>(InputModel input, InvocationContext invocationContext) => input;

        private static Symbol GetSymbolForCurrentCommand(InvocationContext context, string name)
        {
            var executedCommand = context.ParseResult.CommandResult.Symbol;
            Symbol matchingSymbolResult = executedCommand.Children.FirstOrDefault(symbol => IsAliasMatch(symbol, name));
            return matchingSymbolResult;
        }

        public static IPropertyBinder<InputModel> FromName<InputModel, TProperty>(string name, Func<InputModel, TProperty, InputModel> setter)
        {
            return PropertyBinder.FromFunc((InputModel inputModel, InvocationContext context) =>
            {
                
                IPropertyBinder<InputModel> mapFn;
                var symbol = GetSymbolForCurrentCommand(context, name);
                if (symbol == null) throw new ArgumentException($"No input symbol for alias {name}");
                else if (symbol is Argument<TProperty> argRef) mapFn = FromReference(argRef, setter);
                else if (symbol is Option<TProperty> optRef) mapFn = FromReference(optRef, setter);
                else throw new ArgumentException($"Symbol with {name} is not an Option<{typeof(TProperty)} or Arugument<{typeof(TProperty)}>");
                
                return mapFn.Bind(inputModel, context);
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
            return PropertyBinder.FromFunc((InputModel inputModel, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForOption(optionRef);
                return setter(inputModel, propertyValue);
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
            return PropertyBinder.FromFunc((InputModel inputModel, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForArgument(argumentRef);
                return setter(inputModel, propertyValue);
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
