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

    public static class PropertyMap
    {
        private static bool IsAliasMatch(ISymbol symbol, string alias)
        {
            //NOTE: Arguments are not an IdentifierSymbol and just Symbol doesn't have aliases
            return symbol switch
            {
                Option option => option.HasAlias(alias),
                Argument arg => arg.Name == alias,
                _ => false
            };

        }
        private static InputContract NullPropertySetter<InputContract>(InputContract input, InvocationContext invocationContext) => input;

        public static Func<InputContract, InvocationContext, InputContract> FromName<InputContract, TProperty>(string name, Func<InputContract, TProperty, InputContract> setter)
        {
            return (InputContract inputContract, InvocationContext context) =>
            {
                // how are aliases handled?
                var executedCommand = context.ParseResult.CommandResult.Symbol;
                Symbol? matchingSymbolResult = executedCommand.Children.FirstOrDefault(symbol => IsAliasMatch(symbol, name));

                Func<InputContract, InvocationContext, InputContract> mapFn = matchingSymbolResult switch
                {
                    null => throw new ArgumentException($"No input symbol for alias {name}"),
                    Argument<TProperty> argRef => FromReference(argRef, setter),
                    Option<TProperty> argRef => FromReference(argRef, setter),
                    _ => throw new ArgumentException($"Symbol with {name} is not an Option<{typeof(TProperty)} or Arugument<{typeof(TProperty)}>")
                };
                return mapFn(inputContract, context);
            };
        }

        public static Func<InputContract, InvocationContext, InputContract> FromReference<InputContract, TProperty>(Option<TProperty> optionRef, Func<InputContract, TProperty, InputContract> setter)
        {
            return (InputContract inputContract, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForOption<TProperty>(optionRef);
                return setter(inputContract, propertyValue);
            };
        }
        public static Func<InputContract, InvocationContext, InputContract> FromReference<InputContract, TProperty>(Argument<TProperty> argumentRef, Func<InputContract, TProperty, InputContract> setter)
        {
            return (InputContract inputContract, InvocationContext context) =>
            {
                TProperty propertyValue = context.ParseResult.GetValueForArgument<TProperty>(argumentRef);
                return setter(inputContract, propertyValue);
            };
        }
    }

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

    internal class AnonymousCommandHandler : ICommandHandler
    {
        //hmm, this mostly seems to handle sync/async and func/action cases.
        // Probably reasonable to modify it into my own case without duplication concerns
        private readonly Func<InvocationContext, Task> _handle;

        public AnonymousCommandHandler(Func<InvocationContext, Task> handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public AnonymousCommandHandler(Action<InvocationContext> handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            _handle = Handle;

            Task Handle(InvocationContext context)
            {
                handle(context);
                return Task.FromResult(context.ExitCode);
            }
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            object returnValue = _handle(context);

            int ret;

            switch (returnValue)
            {
                case Task<int> exitCodeTask:
                    ret = await exitCodeTask;
                    break;
                case Task task:
                    await task;
                    ret = context.ExitCode;
                    break;
                case int exitCode:
                    ret = exitCode;
                    break;
                default:
                    ret = context.ExitCode;
                    break;
            }

            return ret;
        }
    }
}
