using System.CommandLine.Invocation;
namespace System.CommandLine.PropertyMapBinder
{
    public static partial class ModelFactory
    {
        public static IModelFactory<InputModel> FromContext<InputModel>(Func<InvocationContext, InputModel> factory)
        {
            return new FuncModelFactory<InputModel>(factory);
        }

        private static void EnsureAllSymbolMapsAreValid(InvocationContext context, Symbol[] symbols)
        {
            var command = InvocationContextHelpers.GetCurrentCommand(context);
            foreach(Symbol symbol in symbols){
                if(!InvocationContextHelpers.IsSymbolRegisteredOnCommand(command, symbol)){
                    throw InvocationContextHelpers.MappedSymbolDoesntExist(command, symbol.Name);
                }
            }
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1,InputModel>(Func<T1,InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context =>{
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                return factory(arg1);
            });
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1,T2,InputModel>(Func<T1,T2,InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context =>{
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                var arg2 = InvocationContextHelpers.GetValueForSymbol<T2>(context, symbols[1]);
                return factory(arg1, arg2);
            });
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1,T2,T3,InputModel>(Func<T1,T2,T3,InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context =>{
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                var arg2 = InvocationContextHelpers.GetValueForSymbol<T2>(context, symbols[1]);
                var arg3 = InvocationContextHelpers.GetValueForSymbol<T3>(context, symbols[2]);
                return factory(arg1, arg2, arg3);
            });
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1, T2, T3, T4, InputModel>(Func<T1, T2, T3, T4, InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context => {
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                var arg2 = InvocationContextHelpers.GetValueForSymbol<T2>(context, symbols[1]);
                var arg3 = InvocationContextHelpers.GetValueForSymbol<T3>(context, symbols[2]);
                var arg4 = InvocationContextHelpers.GetValueForSymbol<T4>(context, symbols[3]);
                return factory(arg1, arg2, arg3, arg4);
            });
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1, T2, T3, T4, T5, InputModel>(Func<T1, T2, T3, T4, T5, InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context => {
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                var arg2 = InvocationContextHelpers.GetValueForSymbol<T2>(context, symbols[1]);
                var arg3 = InvocationContextHelpers.GetValueForSymbol<T3>(context, symbols[2]);
                var arg4 = InvocationContextHelpers.GetValueForSymbol<T4>(context, symbols[3]);
                var arg5 = InvocationContextHelpers.GetValueForSymbol<T5>(context, symbols[4]);
                return factory(arg1, arg2, arg3, arg4, arg5);
            });
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1, T2, T3, T4, T5, T6, InputModel>(Func<T1, T2, T3, T4, T5, T6, InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context => {
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                var arg2 = InvocationContextHelpers.GetValueForSymbol<T2>(context, symbols[1]);
                var arg3 = InvocationContextHelpers.GetValueForSymbol<T3>(context, symbols[2]);
                var arg4 = InvocationContextHelpers.GetValueForSymbol<T4>(context, symbols[3]);
                var arg5 = InvocationContextHelpers.GetValueForSymbol<T5>(context, symbols[4]);
                var arg6 = InvocationContextHelpers.GetValueForSymbol<T6>(context, symbols[5]);
                return factory(arg1, arg2, arg3, arg4, arg5, arg6);
            });
        }

        public static IModelFactory<InputModel> FromSymbolMap<T1, T2, T3, T4, T5, T6, T7, InputModel>(Func<T1, T2, T3, T4, T5, T6, T7, InputModel> factory, params Symbol[] symbols)
        {
            return FromContext(context => {
                EnsureAllSymbolMapsAreValid(context, symbols);

                var arg1 = InvocationContextHelpers.GetValueForSymbol<T1>(context, symbols[0]);
                var arg2 = InvocationContextHelpers.GetValueForSymbol<T2>(context, symbols[1]);
                var arg3 = InvocationContextHelpers.GetValueForSymbol<T3>(context, symbols[2]);
                var arg4 = InvocationContextHelpers.GetValueForSymbol<T4>(context, symbols[3]);
                var arg5 = InvocationContextHelpers.GetValueForSymbol<T5>(context, symbols[4]);
                var arg6 = InvocationContextHelpers.GetValueForSymbol<T6>(context, symbols[5]);
                var arg7 = InvocationContextHelpers.GetValueForSymbol<T7>(context, symbols[6]);
                return factory(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            });
        }
    }
}