using System.CommandLine.Invocation;
namespace System.CommandLine.PropertyMapBinder
{
    public interface IModelFactory<T>
    {
        T Create(InvocationContext context);
    }
}