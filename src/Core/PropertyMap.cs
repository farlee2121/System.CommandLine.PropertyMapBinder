using System.CommandLine.Invocation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.CommandLine.PropertyMapBinder
{
    public static partial class PropertyMap
    {
        private static InputModel NullPropertySetter<InputModel>(InputModel input, InvocationContext invocationContext) => input;

        public static IPropertyBinder<InputModel> FromName<InputModel, TProperty>(string name, Func<InputModel, TProperty, InputModel> setter)
        {
            return PropertyBinder.FromFunc((InputModel inputModel, InvocationContext context) =>
            {
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                IPropertyBinder<InputModel> mapFn;
                var symbol = InvocationContextHelpers.GetSymbolForCommand(command, name);
                if (symbol == null) throw InvocationContextHelpers.MappedSymbolDoesntExist(command, name);
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
                var command = InvocationContextHelpers.GetCurrentCommand(context);
                if(!InvocationContextHelpers.IsSymbolRegisteredOnCommand(command, optionRef)) throw InvocationContextHelpers.MappedSymbolDoesntExist(command, string.Join(",", optionRef.Aliases));
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
                var command = InvocationContextHelpers.GetCurrentCommand(context);
                if (!InvocationContextHelpers.IsSymbolRegisteredOnCommand(command, argumentRef)) throw InvocationContextHelpers.MappedSymbolDoesntExist(command, argumentRef.Name);
                
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
