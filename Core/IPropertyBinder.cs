using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    public interface IPropertyBinder<InputContract>
    {
        InputContract Bind(InputContract inputContract, InvocationContext context);
    }
}
