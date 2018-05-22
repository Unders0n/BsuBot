using System;
using System.Threading.Tasks;
using Autofac;
using BsuBot.Logger;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using NLog;

namespace BsuBot.Extensions
{
    /// <summary>
    /// Wrapper for dialog with automatic exception handling, logging and returning (if needed) to user
    /// </summary>
    /// <typeparam name="T">type of dialog</typeparam>
    [Serializable]
    public class ExceptionHandlerDialog<T> : IDialog<T>
    {
        private readonly IDialog<T> _dialog;
        private readonly bool _displayException;
        private readonly int _stackTraceLength;
        [NonSerialized]
        private ILoggerService<ILogger> _loggerService;

        public ExceptionHandlerDialog(IDialog<T> dialog, bool displayException = true, int stackTraceLength = 500)
        {
            _dialog = dialog;
            _displayException = displayException;
            _stackTraceLength = stackTraceLength;
        }

        public async Task StartAsync(IDialogContext context)
        {
            try
            {
                context.Call(_dialog, ResumeAsync);
            }
            catch (Exception e)
            {
                if (_displayException)
                    await DisplayException(context, e).ConfigureAwait(false);
            }
        }

        private async Task ResumeAsync(IDialogContext context, IAwaitable<T> result)
        {
            try
            {
                //resolving loggerService
                using (
                    var scope = DialogModule.BeginLifetimeScope(Conversation.Container, context.Activity.AsMessageActivity())
                )
                {
                    _loggerService = scope.Resolve<ILoggerService<NLog.ILogger>>();
                }


                context.Done(await result);
            }
            catch (Exception e)
            {
                //log exception
                _loggerService.Error(e);

                if (_displayException)
                    await DisplayException(context, e).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Show error message back to channel
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task DisplayException(IDialogContext context, Exception e)
        {

            var stackTrace = e.StackTrace;
            if (stackTrace.Length > _stackTraceLength)
                stackTrace = stackTrace.Substring(0, _stackTraceLength) + "…";
            stackTrace = stackTrace.Replace(Environment.NewLine, "  \n");

            var message = e.Message.Replace(Environment.NewLine, "  \n");

            var inner = e.InnerException?.Message.Replace(Environment.NewLine, "  \n");

            var exceptionStr = $"**{message}**  \n\n{stackTrace} \n\nInner: {inner}";

            await context.PostAsync(exceptionStr).ConfigureAwait(false);
        }
    }
}
