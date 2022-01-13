using System.CommandLine.Invocation;
namespace System.CommandLine.PropertyMapBinder
{
    public class FuncModelFactory<InputModel> : IModelFactory<InputModel>
    {
        private readonly Func<InvocationContext, InputModel> factory;

        public FuncModelFactory(Func<InvocationContext, InputModel> factory)
        {
            this.factory = factory;
        }

        public InputModel Create(InvocationContext context)
        {
            return factory(context);
        }
    }
}