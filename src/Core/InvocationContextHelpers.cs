using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

internal class InvocationContextHelpers
{

    private static bool IsAliasMatch(ISymbol symbol, string alias)
    {
        //NOTE: Arguments are not an IdentifierSymbol and just Symbol doesn't have aliases
        if (symbol is Option option) return option.HasAlias(alias);
        else if (symbol is Argument arg) return arg.Name == alias;
        else return false;

    }

    internal static Command GetCurrentCommand(InvocationContext content){
        return content.ParseResult.CommandResult.Symbol as Command;
    }

    internal static Symbol GetSymbolForCommand(Command command, string name)
    {
        Symbol matchingSymbolResult = command.Children.FirstOrDefault(symbol => IsAliasMatch(symbol, name));
        return matchingSymbolResult;
    }

    internal static bool IsSymbolRegisteredOnCommand(Command command, Symbol symbol)
    {
        return command.Children.Contains(symbol);
    }

    internal static ArgumentException MappedSymbolDoesntExist(Command command, string symbolName)
        => new ArgumentException($"Command {command.Name} has no option or argument for alias {symbolName}");

    internal static T GetValueForSymbol<T>(InvocationContext context, Symbol symbol){
        T value;
        if(symbol is Option<T> opt) value = context.ParseResult.GetValueForOption(opt);
        else if (symbol is Argument<T> arg) value = context.ParseResult.GetValueForArgument(arg);
        else value = default(T);

        return value;
    }
}