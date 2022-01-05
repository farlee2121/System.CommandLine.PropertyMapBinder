using System.CommandLine.Invocation;


namespace System.CommandLine.PropertyMapBinder
{
    internal class AnonymousCommandHandler : ICommandHandler
    {
        //hmm, this mostly seems to handle sync/async and func/action cases.
        // Probably reasonable to modify it into my own case without duplication concerns
        private readonly Func<InvocationContext, Task> _handle;

        public AnonymousCommandHandler(Func<InvocationContext, Task> handle)
        {
            _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        }

        public AnonymousCommandHandler(Action<InvocationContext> handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            _handle = Handle;

            Task Handle(InvocationContext context)
            {
                handle(context);
                return Task.FromResult(context.ExitCode);
            }
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            object returnValue = _handle(context);

            int ret;

            switch (returnValue)
            {
                case Task<int> exitCodeTask:
                    ret = await exitCodeTask;
                    break;
                case Task task:
                    await task;
                    ret = context.ExitCode;
                    break;
                case int exitCode:
                    ret = exitCode;
                    break;
                default:
                    ret = context.ExitCode;
                    break;
            }

            return ret;
        }
    }
}
