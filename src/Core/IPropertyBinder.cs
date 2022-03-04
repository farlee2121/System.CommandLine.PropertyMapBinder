using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    public interface IPropertyBinder<InputModel>
    {
        InputModel Bind(InputModel inputModel, InvocationContext context);
    }
}
