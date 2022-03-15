using System.CommandLine.Invocation;
namespace System.CommandLine.PropertyMapBinder
{
    public class FuncModelFactory<TInputModel> : IModelFactory<TInputModel>
    {
        private readonly Func<InvocationContext, TInputModel> factory;

        public FuncModelFactory(Func<InvocationContext, TInputModel> factory)
        {
            this.factory = factory;
        }

        public TInputModel Create(InvocationContext context)
        {
            return factory(context);
        }
    }
}