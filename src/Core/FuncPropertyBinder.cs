using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    internal class FuncPropertyBinder<TInputModel> : IPropertyBinder<TInputModel>
    {
        private readonly Func<TInputModel, InvocationContext, TInputModel> setter;

        public FuncPropertyBinder(Func<TInputModel, InvocationContext, TInputModel> setter)
        {
            this.setter = setter;
        }
        public TInputModel Bind(TInputModel InputModel, InvocationContext context)
        {
            return setter(InputModel, context);
        }
    }

    internal static class PropertyBinder
    {
        //TODO: Consider how these constructors should be made available. Putting them all in PropertyBinder like it's a module 
        // may not be intuitive for C# users
        public static FuncPropertyBinder<TInputModel> FromFunc<TInputModel>(Func<TInputModel, InvocationContext, TInputModel> setter)
        {
            return new FuncPropertyBinder<TInputModel>(setter);
        }
    }
}
