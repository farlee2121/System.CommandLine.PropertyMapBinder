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
        public static ICommandHandler FromPropertyMap<InputContract>(Action<InputContract> handler, IEnumerable<Func<InputContract, InvocationContext, InputContract>> setters) where InputContract : new()
        {
            
            return new AnonymousCommandHandler(context =>
            {
                InputContract inputContract = new();
                InputContract filledContract = setters.Aggregate(inputContract, (aggContract, setter) => setter(aggContract, context));

                handler(filledContract);
            });
        }

        public static ICommandHandler FromPropertyMap<InputContract, T>(Func<InputContract, T> handler, IEnumerable<Func<InputContract, InvocationContext, InputContract>> setters) where InputContract : new()
        {

            return new AnonymousCommandHandler(context =>
            {
                InputContract inputContract = new();
                InputContract filledContract = setters.Aggregate(inputContract, (aggContract, setter) => setter(aggContract, context));

                handler(filledContract);
            });
        }

    }
}
