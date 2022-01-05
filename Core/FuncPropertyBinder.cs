using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    public class FuncPropertyBinder<InputContract> : IPropertyBinder<InputContract>
    {
        private readonly Func<InputContract, InvocationContext, InputContract> setter;

        public FuncPropertyBinder(Func<InputContract, InvocationContext, InputContract> setter)
        {
            this.setter = setter;
        }
        public InputContract Bind(InputContract inputContract, InvocationContext context)
        {
            return setter(inputContract, context);
        }
    }

    public static class PropertyBinder
    {
        //TODO: Consider how these constructors should be made available. Putting them all in PropertyBinder like it's a module 
        // may not be intuitive for C# users
        public static FuncPropertyBinder<InputContract> FromFunc<InputContract>(Func<InputContract, InvocationContext, InputContract> setter)
        {
            return new FuncPropertyBinder<InputContract>(setter);
        }
    }
}
