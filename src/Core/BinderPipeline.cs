using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Linq.Expressions;

namespace System.CommandLine.PropertyMapBinder
{

    

    public class BinderPipeline<InputModel> : IPropertyBinder<InputModel>, IEnumerable<IPropertyBinder<InputModel>>
    {
        private List<IPropertyBinder<InputModel>> _setters = new List<IPropertyBinder<InputModel>>();

        public BinderPipeline()
        {

        }

        public BinderPipeline(IEnumerable<IPropertyBinder<InputModel>> binders)
        {
            if (binders != null) _setters = binders.ToList();
        }
        //TODO: constructor and enumerable nicities
        public InputModel Bind(InputModel inputModel, InvocationContext context)
        {
            InputModel filledContract = _setters.Aggregate(inputModel, (aggContract, setter) => setter.Bind(aggContract, context));

            return filledContract;
        }

        public void Add(IPropertyBinder<InputModel> propertyBinder)
        {
            _setters.Add(propertyBinder);
        }

        public IEnumerator<IPropertyBinder<InputModel>> GetEnumerator()
        {
            return _setters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _setters.GetEnumerator();
        }

    }

    public static class BinderPipelineExtensions
    {
        public static BinderPipeline<InputModel> MapFromName<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, string name, Func<InputModel, TProperty, InputModel> setter)
        {
            pipeline.Add(new SymbolNamePropertyBinder<InputModel, TProperty>(name, setter));
            return pipeline;
        }

        public static BinderPipeline<InputModel> MapFromName<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, string name, Expression<Func<InputModel, TProperty>> selector)
        {
            pipeline.Add(new SymbolNamePropertyBinder<InputModel, TProperty>(name, selector));
            return pipeline;
        }

        public static BinderPipeline<InputModel> MapFromReference<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, Argument<TProperty> argRef, Func<InputModel, TProperty, InputModel> setter)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<InputModel, TProperty>(argRef, setter));
            return pipeline;
        }

        public static BinderPipeline<InputModel> MapFromReference<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, Argument<TProperty> argRef, Expression<Func<InputModel, TProperty>> selector)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<InputModel, TProperty>(argRef, selector));
            return pipeline;
        }

        public static BinderPipeline<InputModel> MapFromReference<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, Option<TProperty> argRef, Func<InputModel, TProperty, InputModel> setter)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<InputModel, TProperty>(argRef, setter));
            return pipeline;
        }


        public static BinderPipeline<InputModel> MapFromReference<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, Option<TProperty> optionRef, Expression<Func<InputModel, TProperty>> selector)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<InputModel, TProperty>(optionRef, selector));
            return pipeline;
        }

        public static BinderPipeline<InputModel> MapFromValue<InputModel, TProperty>(this BinderPipeline<InputModel> pipeline, Expression<Func<InputModel, TProperty>> selector, TProperty value)
        {
            var binder = new ConstantPropertyBinder<InputModel, TProperty>(value, ReflectionHelper.SelectorToSetter(selector));
            pipeline.Add(binder);
            return pipeline;
        }

        public static ICommandHandler ToHandler<InputModel, T>(this BinderPipeline<InputModel> pipeline, Func<InputModel,T> handlerFunc) where InputModel : new()
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline);
        }

        public static ICommandHandler ToHandler<InputModel>(this BinderPipeline<InputModel> pipeline, Action<InputModel> handlerFunc) where InputModel : new()
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline);
        }

        public static ICommandHandler ToHandler<InputModel, T>(this BinderPipeline<InputModel> pipeline, Func<InputModel, T> handlerFunc, InputModel inputModel)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputModel);
        }

        public static ICommandHandler ToHandler<InputModel>(this BinderPipeline<InputModel> pipeline, Action<InputModel> handlerFunc, InputModel inputModel)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputModel);
        }

        public static ICommandHandler ToHandler<InputModel, T>(this BinderPipeline<InputModel> pipeline, Func<InputModel, T> handlerFunc, IModelFactory<InputModel> inputFactory)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputFactory);
        }

        public static ICommandHandler ToHandler<InputModel>(this BinderPipeline<InputModel> pipeline, Action<InputModel> handlerFunc, IModelFactory<InputModel> inputFactory)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputFactory);
        }

    }
}
