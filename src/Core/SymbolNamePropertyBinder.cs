using System.CommandLine.Invocation;
using System.Linq.Expressions;
using System.Reflection;

namespace System.CommandLine.PropertyMapBinder
{
    public class SymbolNamePropertyBinder<TInputModel, TProperty> : IPropertyBinder<TInputModel>
    {
        private readonly string symbolName;
        private readonly Func<TInputModel, TProperty, TInputModel> _propertySetter;

        public SymbolNamePropertyBinder(string symbolName, Func<TInputModel, TProperty, TInputModel> propertySetter)
        {
            this.symbolName = symbolName;
            _propertySetter = propertySetter;
        }

        public SymbolNamePropertyBinder(string symbolName, Expression<Func<TInputModel, TProperty>> selectorLambda)
        {
            this.symbolName = symbolName;
            _propertySetter = SelectorToSetter(selectorLambda);
        }

        private Func<TInputModel, TProperty, TInputModel> SelectorToSetter(Expression<Func<TInputModel, TProperty>> selectorLambda){
            return (contract, propertyValue) =>
            {
                MemberInfo member = ReflectionHelper.FindProperty(selectorLambda);
                ReflectionHelper.SetMemberValue(member, contract, propertyValue);
                return contract;
            };
        }

        public TInputModel Bind(TInputModel inputModel, InvocationContext context)
        {
            var command = InvocationContextHelpers.GetCurrentCommand(context);

            IPropertyBinder<TInputModel> mapFn;
            var symbol = InvocationContextHelpers.GetSymbolForCommand(command, symbolName);
            if (symbol == null) throw InvocationContextHelpers.MappedSymbolDoesntExist(command, symbolName);
            else if (symbol is Argument<TProperty> argRef) mapFn = new SymbolReferencePropertyBinder<TInputModel, TProperty>(argRef, _propertySetter);
            else if (symbol is Option<TProperty> optRef) mapFn = new SymbolReferencePropertyBinder<TInputModel, TProperty>(optRef, _propertySetter);
            else throw new ArgumentException($"Symbol with {symbolName} is not an Option<{typeof(TProperty)} or Arugument<{typeof(TProperty)}>");
            
            return mapFn.Bind(inputModel, context);
        }
    }
}
