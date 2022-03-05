using System.CommandLine.Invocation;
using System.Linq.Expressions;
using System.Reflection;

namespace System.CommandLine.PropertyMapBinder
{
    public class SymbolReferencePropertyBinder<TInputModel, TProperty> : IPropertyBinder<TInputModel>
    {
        private readonly Func<TInputModel, TProperty, TInputModel> _propertySetter;
        private readonly Symbol symbolReference;

        public SymbolReferencePropertyBinder(Argument<TProperty> argumentRef, Func<TInputModel, TProperty, TInputModel> setter)
        {
            symbolReference = argumentRef;
            _propertySetter = setter;
        }

        private Func<TInputModel, TProperty, TInputModel> SelectorToSetter(Expression<Func<TInputModel, TProperty>> selectorLambda){
            return (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            };
        }

        public SymbolReferencePropertyBinder(Argument<TProperty> argumentRef, Expression<Func<TInputModel, TProperty>> selectorLambda)
        {
            symbolReference = argumentRef;
            _propertySetter = SelectorToSetter(selectorLambda);
        }

        public SymbolReferencePropertyBinder(Option<TProperty> optionRef, Func<TInputModel, TProperty, TInputModel> setter)
        {
            symbolReference = optionRef;
            _propertySetter = setter;   
        }

        public SymbolReferencePropertyBinder(Option<TProperty> optionRef, Expression<Func<TInputModel, TProperty>> selectorLambda)
        {
            symbolReference = optionRef;
            _propertySetter = SelectorToSetter(selectorLambda);
        }


        public TInputModel Bind(TInputModel inputModel, InvocationContext context)
        {
            var command = InvocationContextHelpers.GetCurrentCommand(context);
            if (!InvocationContextHelpers.IsSymbolRegisteredOnCommand(command, symbolReference)) throw InvocationContextHelpers.MappedSymbolDoesntExist(command, symbolReference.Name);
            
            TProperty propertyValue = InvocationContextHelpers.GetValueForSymbol<TProperty>(context, symbolReference);
            return _propertySetter(inputModel, propertyValue);
        }
    }
}
