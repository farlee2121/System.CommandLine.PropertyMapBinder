using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using CaseExtensions;

namespace System.CommandLine.PropertyMapBinder.NameConventionBinder
{
    public delegate bool NameConventionComparer(IReadOnlyCollection<string> cliAliases, string memberName);

    public static class NameConventions {
        public static NameConventionComparer PascalCaseComparer = (aliases, memberName) =>
        {
            return aliases.Any(a => a.ToPascalCase() == memberName);
        };

        public static NameConventionComparer CamelCaseComparer = (aliases, memberName) =>
        {
            return aliases.Any(a => a.ToCamelCase() == memberName);
        };

        public static NameConventionComparer SnakeCaseComparer = (aliases, memberName) =>
        {
            return aliases.Any(a => a.ToSnakeCase() == memberName);
        };
    }

    public class NameConventionBinder<InputModel> : IPropertyBinder<InputModel>
    {

        private NameConventionComparer NullComparer = (aliases, memberName) => false;

        NameConventionComparer _conventionComparer;

        public NameConventionBinder(NameConventionComparer nameComparer)
        {
            _conventionComparer = nameComparer;
        }

        private object GetSymbolValue(InvocationContext context, Symbol symbol)
        {
            object value;
            if(symbol is Argument arg) value = context.ParseResult.GetValueForArgument(arg);
            else if(symbol is Option opt) value = context.ParseResult.GetValueForOption(opt);
            else value = null;

            return value; 
        }

        private IReadOnlyCollection<string> GetSymbolAliases(Symbol symbol)
        {
            if (symbol is Argument arg) return new[] { arg.Name }.ToReadOnlyCollection();
            else if (symbol is Option opt) return opt.Aliases.ToReadOnlyCollection();
            else return new string[0].ToReadOnlyCollection();
        }

        public InputModel Bind(InputModel inputModel, InvocationContext context)
        {
            var currentCommand = context.ParseResult.CommandResult.Symbol;
            var symbols = currentCommand.Children;

            Type inputModelType = typeof(InputModel);
            var inputModelMembers = new List<MemberInfo>()
                //TODO: probably want to limit to public if this goes beyond proof of concept
                .Concat(inputModelType.GetProperties() ?? Enumerable.Empty<MemberInfo>())
                .Concat(inputModelType.GetFields() ?? Enumerable.Empty<MemberInfo>());

            foreach(Symbol symbol in symbols)
            {
                var symbolAliases = GetSymbolAliases(symbol);
                MemberInfo matchedMember = inputModelMembers.FirstOrDefault(member => _conventionComparer(symbolAliases, member.Name));
                if(matchedMember != null)
                {
                    object parsedValue = GetSymbolValue(context, symbol);
                    if(parsedValue != null) ReflectionHelper.SetMemberValue(matchedMember, inputModel, parsedValue);
                }
            }

            return inputModel;
        }
    }

    public static class ReadOnlyExtensions
    {
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> seq)
        {
            return new System.Collections.ObjectModel.ReadOnlyCollection<T>(seq.ToList());
        }
    }
}
