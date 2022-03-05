using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.Reflection;
using System.CommandLine.Invocation;

namespace System.CommandLine.PropertyMapBinder.CliExample
{
    internal class UserPromptBinder
    {
    }

    public static class UserPropmptBinderPipelineExtensions
    {
        public static BinderPipeline<InputModel> PromptUser<InputModel, T>(this BinderPipeline<InputModel> pipeline, string prompt, Expression<Func<InputModel, T>> selectorLambda)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine();

            var arg = new Argument<T>()
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            var parseResult = arg.Parse(input);

            T parsed = parseResult.GetValueForArgument(arg) ?? default;

            return pipeline.MapFromValue(selectorLambda, parsed);
        }

        public static BinderPipeline<InputModel> PromptIfMissing<InputModel, T>(this BinderPipeline<InputModel> pipeline, string prompt, Expression<Func<InputModel, T>> selectorLambda)
        {
            var binder = FuncPropertyBinder.FromFunc((InputModel model, InvocationContext context) =>
            {
                var modelValue = selectorLambda.Compile().Invoke(model);
                if (modelValue == null)
                {
                    Console.WriteLine(prompt);
                    string input = Console.ReadLine();

                    var arg = new Argument<T>()
                    {
                        Arity = ArgumentArity.ZeroOrOne
                    };
                    var parseResult = arg.Parse(input);

                    T parsed = parseResult.GetValueForArgument(arg) ?? default;

                    var member = ReflectionHelper.FindProperty(selectorLambda);
                    ReflectionHelper.SetMemberValue(member, model, parsed);
                }

                return model;
            });
            pipeline.Add(binder);

            return pipeline;
        }

        public static BinderPipeline<InputModel> PromptIfMissing<InputModel, T>(this BinderPipeline<InputModel> pipeline, Option<T> symbol, Expression<Func<InputModel, T>> selectorLambda)
        {
            //Q: do I really need this scenario. Prompt if missing should really be operating off of the input model because values can come from many sources

            pipeline.MapFromReference(symbol, (model, value) =>
            {
                var modelValue = selectorLambda.Compile().Invoke(model);
                if (value == null && modelValue == null)
                {
                    Console.WriteLine($"Please provide a value for {symbol.Name}");
                    string input = Console.ReadLine();

                    var parseResult = symbol.Parse(input);
                    T parsedValue = parseResult.GetValueForOption(symbol);

                    //TODO: I may want to surface these model operations in the core library
                    var member = ReflectionHelper.FindProperty(selectorLambda);
                    ReflectionHelper.SetMemberValue(member, model, parsedValue);
                }

                return model;

            });
            return pipeline;
        }

    }

    internal class FuncPropertyBinder<InputModel> : IPropertyBinder<InputModel>
    {
        private readonly Func<InputModel, InvocationContext, InputModel> setter;

        public FuncPropertyBinder(Func<InputModel, InvocationContext, InputModel> setter)
        {
            this.setter = setter;
        }
        public InputModel Bind(InputModel InputModel, InvocationContext context)
        {
            return setter(InputModel, context);
        }
    }

    internal static class FuncPropertyBinder
    {
        //TODO: Consider how these constructors should be made available. Putting them all in PropertyBinder like it's a module 
        // may not be intuitive for C# users
        public static FuncPropertyBinder<InputModel> FromFunc<InputModel>(Func<InputModel, InvocationContext, InputModel> setter)
        {
            return new FuncPropertyBinder<InputModel>(setter);
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

        public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression;

            var done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = ((LambdaExpression)expressionToCheck).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = ((MemberExpression)expressionToCheck);

                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException(
                                $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
                                nameof(lambdaExpression));
                        }

                        var member = memberExpression.Member;

                        return member;
                    default:
                        done = true;
                        break;
                }
            }

            throw new InvalidOperationException("Couldn't resolve a member from given selector.");
        }
    }
}
