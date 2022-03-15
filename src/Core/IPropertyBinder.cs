using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    public interface IPropertyBinder<TInputModel>
    {
        TInputModel Bind(TInputModel inputModel, InvocationContext context);
    }
}
