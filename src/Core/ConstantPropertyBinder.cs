using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Text;

namespace System.CommandLine.PropertyMapBinder
{
    internal class ConstantPropertyBinder<TInputModel, TProperty> : IPropertyBinder<TInputModel>
    {
        private readonly TProperty value;
        private readonly Func<TInputModel, TProperty, TInputModel> _propertySetter;

        public ConstantPropertyBinder(TProperty value, Func<TInputModel, TProperty, TInputModel> setter)
        {
            this.value = value;
            _propertySetter = setter;
        }
        public TInputModel Bind(TInputModel inputModel, InvocationContext context)
        {
            return _propertySetter(inputModel, value);
        }
    }
}
