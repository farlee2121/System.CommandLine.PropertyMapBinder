using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{

    internal class CommandHandler
    {
        public static ICommandHandler FromPropertyMap<TInputModel>(Action<TInputModel> handler, IPropertyBinder<TInputModel> propertyBinder) where TInputModel : new()
        {
            
            return new AnonymousCommandHandler(context =>
            {
                TInputModel inputModel = new TInputModel();
                TInputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<TInputModel, T>(Func<TInputModel, T> handler, IPropertyBinder<TInputModel> propertyBinder) where TInputModel : new()
        {
            return new AnonymousCommandHandler(context =>
            {
                TInputModel inputModel = new TInputModel();
                TInputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<TInputModel, T>(Func<TInputModel, T> handler, IPropertyBinder<TInputModel> propertyBinder, TInputModel inputModel)
        {
            return new AnonymousCommandHandler(context =>
            {
                TInputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<TInputModel>(Action<TInputModel> handler, IPropertyBinder<TInputModel> propertyBinder, TInputModel inputModel)
        {
            return new AnonymousCommandHandler(context =>
            {
                TInputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<TInputModel, T>(Func<TInputModel, T> handler, IPropertyBinder<TInputModel> propertyBinder, IModelFactory<TInputModel> inputFactory)
        {
            return new AnonymousCommandHandler(context =>
            {
                var inputModel = inputFactory.Create(context);
                TInputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<TInputModel>(Action<TInputModel> handler, IPropertyBinder<TInputModel> propertyBinder, IModelFactory<TInputModel> inputFactory)
        {
            return new AnonymousCommandHandler(context =>
            {
                var inputModel = inputFactory.Create(context);
                TInputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

    }
}
