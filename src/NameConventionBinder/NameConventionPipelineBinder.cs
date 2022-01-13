using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reflection;
using CaseExtensions;

namespace System.CommandLine.PropertyMapBinder.NameConventionBinder
{
    internal class NameConventionPipelineBinder<InputModel> : IPropertyBinder<InputModel>
    {
        TextCase _allowedNameCase;
        public NameConventionPipelineBinder(TextCase allowedNameCasing)
        {
            _allowedNameCase = allowedNameCasing;
        }

        private string ToCase(string str, TextCase casing)
        {
            string caseAdjusted;
            if(casing == TextCase.Camel) caseAdjusted = str.ToCamelCase();
            else if(casing == TextCase.Pascal) caseAdjusted = str.ToPascalCase();
            else if(casing == TextCase.Snake) caseAdjusted = str.ToSnakeCase();
            else if(casing == TextCase.Train) caseAdjusted = str.ToTrainCase();
            else caseAdjusted = str;

            return caseAdjusted;
        }

        private object GetSymbolValue(InvocationContext context, Symbol symbol)
        {
            object value;
            if(symbol is Argument arg) value = context.ParseResult.GetValueForArgument(arg);
            else if(symbol is Option opt) value = context.ParseResult.GetValueForOption(opt);
            else value = null;

            return value; 
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
                MemberInfo matchedMember = inputModelMembers.FirstOrDefault(member => member.Name == normalizedSymbolName);
                if(matchedMember != null)
                {
                    object parsedValue = GetSymbolValue(context, symbol);
                    if(parsedValue != null) ReflectionHelper.SetMemberValue(matchedMember, inputModel, parsedValue);
                }
            }

            return inputModel;
        }
    }
}
