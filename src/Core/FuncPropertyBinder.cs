using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    public class FuncPropertyBinder<InputModel> : IPropertyBinder<InputModel>
    {
        private readonly Func<InputModel, InvocationContext, InputModel> setter;

        public FuncPropertyBinder(Func<InputModel, InvocationContext, InputModel> setter)
        {
            this.setter = setter;
        }
        public InputModel Bind(InputModel InputModel, InvocationContext context)
        {
            return setter(InputModel, context);
        }
    }

    public static class PropertyBinder
    {
        //TODO: Consider how these constructors should be made available. Putting them all in PropertyBinder like it's a module 
        // may not be intuitive for C# users
        public static FuncPropertyBinder<InputModel> FromFunc<InputModel>(Func<InputModel, InvocationContext, InputModel> setter)
        {
            return new FuncPropertyBinder<InputModel>(setter);
        }
    }
}
