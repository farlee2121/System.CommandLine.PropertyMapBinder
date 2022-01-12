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

    public class CommandHandler
    {
        // IDEA: TODO: assuming a default constructor is a bit dangerous. I should probably allow some way to provide the base config instance or a factory (context) => base instance 
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

    }
}
