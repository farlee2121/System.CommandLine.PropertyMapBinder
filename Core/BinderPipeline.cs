﻿using System.Collections;
using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{

    

    public class BinderPipeline<InputContract> : IPropertyBinder<InputContract>, IEnumerable<IPropertyBinder<InputContract>>
    {
        private List<IPropertyBinder<InputContract>> _setters = new ();

        public BinderPipeline()
        {

        }

        public BinderPipeline(IEnumerable<IPropertyBinder<InputContract>> binders)
        {
            if (binders != null) _setters = binders.ToList();
        }
        //TODO: constructor and enumerable nicities
        public InputContract Bind(InputContract inputContract, InvocationContext context)
        {
            InputContract filledContract = _setters.Aggregate(inputContract, (aggContract, setter) => setter.Bind(aggContract, context));

            return filledContract;
        }

        public void Add(IPropertyBinder<InputContract> propertyBinder)
        {
            _setters.Add(propertyBinder);
        }

        public IEnumerator<IPropertyBinder<InputContract>> GetEnumerator()
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
        public static BinderPipeline<InputContract> AddByName<InputContract, TProperty>(this BinderPipeline<InputContract> pipeline, string name, Func<InputContract, TProperty, InputContract> setter)
        {
            pipeline.Add(PropertyMap.FromName(name, setter));
            return pipeline;
        }

        public static BinderPipeline<InputContract> AddByReference<InputContract, TProperty>(this BinderPipeline<InputContract> pipeline, Argument<TProperty> argRef, Func<InputContract, TProperty, InputContract> setter)
        {
            pipeline.Add(PropertyMap.FromReference(argRef, setter));
            return pipeline;
        }
        public static BinderPipeline<InputContract> AddByReference<InputContract, TProperty>(this BinderPipeline<InputContract> pipeline, Option<TProperty> argRef, Func<InputContract, TProperty, InputContract> setter)
        {
            pipeline.Add(PropertyMap.FromReference(argRef, setter));
            return pipeline;
        }
    }
}
