using System.Collections;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.PropertyMapBinder.PropertyBinders;
using System.Linq;
using System.Linq.Expressions;

namespace System.CommandLine.PropertyMapBinder
{

    public class BinderPipeline<TInputModel> : IPropertyBinder<TInputModel>, IEnumerable<IPropertyBinder<TInputModel>>
    {
        private List<IPropertyBinder<TInputModel>> _setters = new List<IPropertyBinder<TInputModel>>();

        public BinderPipeline()
        {

        }

        public BinderPipeline(IEnumerable<IPropertyBinder<TInputModel>> binders)
        {
            if (binders != null) _setters = binders.ToList();
        }
        //TODO: constructor and enumerable nicities
        public TInputModel Bind(TInputModel inputModel, InvocationContext context)
        {
            TInputModel filledContract = _setters.Aggregate(inputModel, (aggContract, setter) => setter.Bind(aggContract, context));

            return filledContract;
        }

        public void Add(IPropertyBinder<TInputModel> propertyBinder)
        {
            _setters.Add(propertyBinder);
        }

        public IEnumerator<IPropertyBinder<TInputModel>> GetEnumerator()
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
        public static BinderPipeline<TInputModel> MapFromName<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, string name, Func<TInputModel, TProperty, TInputModel> setter)
        {
            pipeline.Add(new SymbolNamePropertyBinder<TInputModel, TProperty>(name, setter));
            return pipeline;
        }

        public static BinderPipeline<TInputModel> MapFromName<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, string name, Expression<Func<TInputModel, TProperty>> selector)
        {
            pipeline.Add(new SymbolNamePropertyBinder<TInputModel, TProperty>(name, selector));
            return pipeline;
        }

        public static BinderPipeline<TInputModel> MapFromReference<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, Argument<TProperty> argRef, Func<TInputModel, TProperty, TInputModel> setter)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<TInputModel, TProperty>(argRef, setter));
            return pipeline;
        }

        public static BinderPipeline<TInputModel> MapFromReference<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, Argument<TProperty> argRef, Expression<Func<TInputModel, TProperty>> selector)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<TInputModel, TProperty>(argRef, selector));
            return pipeline;
        }

        public static BinderPipeline<TInputModel> MapFromReference<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, Option<TProperty> argRef, Func<TInputModel, TProperty, TInputModel> setter)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<TInputModel, TProperty>(argRef, setter));
            return pipeline;
        }


        public static BinderPipeline<TInputModel> MapFromReference<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, Option<TProperty> optionRef, Expression<Func<TInputModel, TProperty>> selector)
        {
            pipeline.Add(new SymbolReferencePropertyBinder<TInputModel, TProperty>(optionRef, selector));
            return pipeline;
        }

        public static BinderPipeline<TInputModel> MapFromValue<TInputModel, TProperty>(this BinderPipeline<TInputModel> pipeline, Expression<Func<TInputModel, TProperty>> selector, TProperty value)
        {
            var binder = new ConstantPropertyBinder<TInputModel, TProperty>(value, ReflectionHelper.SelectorToSetter(selector));
            pipeline.Add(binder);
            return pipeline;
        }

        public static ICommandHandler ToHandler<TInputModel, T>(this BinderPipeline<TInputModel> pipeline, Func<TInputModel,T> handlerFunc) where TInputModel : new()
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline);
        }

        public static ICommandHandler ToHandler<TInputModel>(this BinderPipeline<TInputModel> pipeline, Action<TInputModel> handlerFunc) where TInputModel : new()
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline);
        }

        public static ICommandHandler ToHandler<TInputModel, T>(this BinderPipeline<TInputModel> pipeline, Func<TInputModel, T> handlerFunc, TInputModel inputModel)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputModel);
        }

        public static ICommandHandler ToHandler<TInputModel>(this BinderPipeline<TInputModel> pipeline, Action<TInputModel> handlerFunc, TInputModel inputModel)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputModel);
        }

        public static ICommandHandler ToHandler<TInputModel, T>(this BinderPipeline<TInputModel> pipeline, Func<TInputModel, T> handlerFunc, IModelFactory<TInputModel> inputFactory)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputFactory);
        }

        public static ICommandHandler ToHandler<TInputModel>(this BinderPipeline<TInputModel> pipeline, Action<TInputModel> handlerFunc, IModelFactory<TInputModel> inputFactory)
        {
            return CommandHandler.FromPropertyMap(handlerFunc, pipeline, inputFactory);
        }

    }
}
