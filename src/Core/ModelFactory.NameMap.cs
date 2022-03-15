using System.CommandLine.Invocation;
using System.CommandLine.PropertyMapBinder.Internals;

namespace System.CommandLine.PropertyMapBinder
{
    public static partial class ModelFactory
    {

        private static T GetValueForAliasOrError<T>(InvocationContext context, Command command, string alias)
        {
            Symbol symbol = InvocationContextHelpers.GetSymbolForCommand(command, alias);
            if(symbol == null) throw InvocationContextHelpers.MappedSymbolDoesntExist(command, alias);

            T value = InvocationContextHelpers.GetValueForSymbol<T>(context, symbol);
            return value;
        }
        public static IModelFactory<TInputModel> FromNameMap<T1,TInputModel>(Func<T1,TInputModel> factory, params string[] aliases)
        {
            return FromContext(context =>{
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                return factory(arg1);
            });
        }

        public static IModelFactory<TInputModel> FromNameMap<T1,T2,TInputModel>(Func<T1,T2,TInputModel> factory, params string[] aliases)
        {
            return FromContext(context =>{
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                var arg2 = GetValueForAliasOrError<T2>(context, command, aliases[1]);
                return factory(arg1, arg2);
            });
        }

        public static IModelFactory<TInputModel> FromNameMap<T1,T2,T3,TInputModel>(Func<T1,T2,T3,TInputModel> factory, params string[] aliases)
        {
            return FromContext(context =>{
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                var arg2 = GetValueForAliasOrError<T2>(context, command, aliases[1]);
                var arg3 = GetValueForAliasOrError<T3>(context, command, aliases[2]);
                return factory(arg1, arg2, arg3);
            });
        }

        public static IModelFactory<TInputModel> FromNameMap<T1, T2, T3, T4, TInputModel>(Func<T1, T2, T3, T4, TInputModel> factory, params string[] aliases)
        {
            return FromContext(context => {
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                var arg2 = GetValueForAliasOrError<T2>(context, command, aliases[1]);
                var arg3 = GetValueForAliasOrError<T3>(context, command, aliases[2]);
                var arg4 = GetValueForAliasOrError<T4>(context, command, aliases[3]);
                return factory(arg1, arg2, arg3, arg4);
            });
        }

        public static IModelFactory<TInputModel> FromNameMap<T1, T2, T3, T4, T5, TInputModel>(Func<T1, T2, T3, T4, T5, TInputModel> factory, params string[] aliases)
        {
            return FromContext(context => {
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                var arg2 = GetValueForAliasOrError<T2>(context, command, aliases[1]);
                var arg3 = GetValueForAliasOrError<T3>(context, command, aliases[2]);
                var arg4 = GetValueForAliasOrError<T4>(context, command, aliases[3]);
                var arg5 = GetValueForAliasOrError<T5>(context, command, aliases[4]);
                return factory(arg1, arg2, arg3, arg4, arg5);
            });
        }

        public static IModelFactory<TInputModel> FromNameMap<T1, T2, T3, T4, T5, T6, TInputModel>(Func<T1, T2, T3, T4, T5, T6, TInputModel> factory, params string[] aliases)
        {
            return FromContext(context => {
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                var arg2 = GetValueForAliasOrError<T2>(context, command, aliases[1]);
                var arg3 = GetValueForAliasOrError<T3>(context, command, aliases[2]);
                var arg4 = GetValueForAliasOrError<T4>(context, command, aliases[3]);
                var arg5 = GetValueForAliasOrError<T5>(context, command, aliases[4]);
                var arg6 = GetValueForAliasOrError<T6>(context, command, aliases[5]);
                return factory(arg1, arg2, arg3, arg4, arg5, arg6);
            });
        }

        public static IModelFactory<TInputModel> FromNameMap<T1, T2, T3, T4, T5, T6, T7, TInputModel>(Func<T1, T2, T3, T4, T5, T6, T7, TInputModel> factory, params string[] aliases)
        {
            return FromContext(context => {
                var command = InvocationContextHelpers.GetCurrentCommand(context);

                var arg1 = GetValueForAliasOrError<T1>(context, command, aliases[0]);
                var arg2 = GetValueForAliasOrError<T2>(context, command, aliases[1]);
                var arg3 = GetValueForAliasOrError<T3>(context, command, aliases[2]);
                var arg4 = GetValueForAliasOrError<T4>(context, command, aliases[3]);
                var arg5 = GetValueForAliasOrError<T5>(context, command, aliases[4]);
                var arg6 = GetValueForAliasOrError<T6>(context, command, aliases[5]);
                var arg7 = GetValueForAliasOrError<T7>(context, command, aliases[6]);
                return factory(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            });
        }
    }
}