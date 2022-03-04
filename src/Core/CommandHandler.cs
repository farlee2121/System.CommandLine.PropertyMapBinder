using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System.CommandLine.PropertyMapBinder
{

    internal class CommandHandler
    {
        public static ICommandHandler FromPropertyMap<InputModel>(Action<InputModel> handler, IPropertyBinder<InputModel> propertyBinder) where InputModel : new()
        {
            
            return new AnonymousCommandHandler(context =>
            {
                InputModel inputModel = new InputModel();
                InputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<InputModel, T>(Func<InputModel, T> handler, IPropertyBinder<InputModel> propertyBinder) where InputModel : new()
        {
            return new AnonymousCommandHandler(context =>
            {
                InputModel inputModel = new InputModel();
                InputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<InputModel, T>(Func<InputModel, T> handler, IPropertyBinder<InputModel> propertyBinder, InputModel inputModel)
        {
            return new AnonymousCommandHandler(context =>
            {
                InputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<InputModel>(Action<InputModel> handler, IPropertyBinder<InputModel> propertyBinder, InputModel inputModel)
        {
            return new AnonymousCommandHandler(context =>
            {
                InputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<InputModel, T>(Func<InputModel, T> handler, IPropertyBinder<InputModel> propertyBinder, IModelFactory<InputModel> inputFactory)
        {
            return new AnonymousCommandHandler(context =>
            {
                var inputModel = inputFactory.Create(context);
                InputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<InputModel>(Action<InputModel> handler, IPropertyBinder<InputModel> propertyBinder, IModelFactory<InputModel> inputFactory)
        {
            return new AnonymousCommandHandler(context =>
            {
                var inputModel = inputFactory.Create(context);
                InputModel filledContract = propertyBinder.Bind(inputModel, context);
                handler(filledContract);
            });
        }

    }
}
