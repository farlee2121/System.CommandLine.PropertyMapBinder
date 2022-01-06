using System;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using CaseExtensions;

namespace System.CommandLine.PropertyMapBinder.CliExample
{
    public enum TextCase
    {
        Camel,
        Pascal,
        Snake,
        Train,
    }

    internal class NamingConventionPipelineBinder<InputModel> : IPropertyBinder<InputModel>
    {
        TextCase _allowedNameCase;
        public NamingConventionPipelineBinder(TextCase allowedNameCasing)
        {
            _allowedNameCase = allowedNameCasing;
        }

        private string ToCase(string str, TextCase casing)
        {
            return casing switch
            {
                TextCase.Camel => str.ToCamelCase(),
                TextCase.Pascal => str.ToPascalCase(),
                TextCase.Snake => str.ToSnakeCase(),
                TextCase.Train => str.ToTrainCase(),
                _ => str
            };
        }

        private object? GetSymbolValue(InvocationContext context, Symbol symbol)
        {
            return symbol switch
            {
                Argument arg => context.ParseResult.GetValueForArgument(arg),
                Option opt => context.ParseResult.GetValueForOption(opt),
                _ => null
            };
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
                string normalizedSymbolName = ToCase(symbol.Name, _allowedNameCase);
                MemberInfo? matchedMember = inputModelMembers.FirstOrDefault(member => member.Name == normalizedSymbolName);
                if(matchedMember != null)
                {
                    object? parsedValue = GetSymbolValue(context, symbol);
                    if(parsedValue != null) ReflectionHelper.SetMemberValue(matchedMember, inputModel, parsedValue);
                }
            }

            return inputModel;
        }
    }

    public static class PipelineBinderExtensions
    {
        public static BinderPipeline<InputModel> MapFromNameConvention<InputModel>(this BinderPipeline<InputModel> pipeline, TextCase casing){
            pipeline.Add(new NamingConventionPipelineBinder<InputModel>(casing));
            return pipeline;
        }
    }


    internal static class ReflectionHelper
    {
        private static ArgumentOutOfRangeException Expected(MemberInfo propertyOrField) => new ArgumentOutOfRangeException(nameof(propertyOrField), "Expected a property or field, not " + propertyOrField);

        public static void SetMemberValue(this MemberInfo propertyOrField, object target, object value)
        {
            if (propertyOrField is PropertyInfo property)
            {
                property.SetValue(target, value, null);
                return;
            }
            if (propertyOrField is FieldInfo field)
            {
                field.SetValue(target, value);
                return;
            }
            throw Expected(propertyOrField);
        }
    }

}

namespace System.CommandLine.PropertyMapBinder
{
    using System.CommandLine.PropertyMapBinder.CliExample;
    public static partial class PropertyMap
    {
        public static IPropertyBinder<InputModel> FromNameConvention<InputModel>(TextCase casing)
        {
            return new NamingConventionPipelineBinder<InputModel>(casing);
        }
    }
}
