using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a UI command in SpectNetIDE
    /// </summary>
    public abstract class SpectNetCommandBase: IDisposable
    {
        private readonly CancellationTokenSource _disposalTokenSource = new CancellationTokenSource();

        protected JoinableTaskFactory JoinableTaskFactory { get; }
        protected JoinableTaskCollection JoinableTaskCollection { get; }

        /// <summary>
        /// Gets a <see cref="CancellationToken"/> that can be used to check if the package has been disposed.
        /// </summary>
        public CancellationToken DisposalToken => _disposalTokenSource.Token;

        /// <summary>
        /// Indicates if the command has been cancelled
        /// </summary>
        public bool IsCancelled { get; protected set; }

        /// <summary>
        /// This flags indicates that the command UI should be 
        /// updated when the command has been completed --
        /// with failure or success
        /// </summary>
        public virtual bool UpdateUiWhenComplete => true;

        /// <summary>
        /// The id of the command
        /// </summary>
        public int CommandId { get; }

        /// <summary>
        /// Obtains the OleMenuCommand behind this command
        /// </summary>
        public OleMenuCommand OleMenuCommand { get; }

        /// <summary>
        /// Sites the instance of this class
        /// </summary>
        /// <remarks>
        /// Commands should be initialized only from the main thread.
        /// </remarks>
        protected SpectNetCommandBase()
        {
            // --- SpectNetIDE uses only async commands
            JoinableTaskCollection = ThreadHelper.JoinableTaskContext.CreateCollection();
            JoinableTaskFactory = ThreadHelper.JoinableTaskContext.CreateFactory(JoinableTaskCollection);

            // --- Obtain and store command properties
            var idAttr = GetType().GetTypeInfo().GetCustomAttribute<CommandIdAttribute>();
            if (idAttr != null)
            {
                CommandId = idAttr.Value;
            }

            // --- Register the command
            var package = SpectNetPackage.Default;
            if (!(((IServiceProvider)package).GetService(
                typeof(IMenuCommandService)) is OleMenuCommandService commandService))
            {
                return;
            }

            var commandId = new CommandID(new Guid(SpectNetPackage.COMMAND_SET_GUID), CommandId);
            OleMenuCommand = new OleMenuCommand((s, e) => { OnExecute(); }, commandId);
            OleMenuCommand.BeforeQueryStatus += (s, e) =>
            {
                if (s is OleMenuCommand mc)
                {
                    OnQueryStatus(mc);
                }
            };
            commandService.AddCommand(OleMenuCommand);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the lion's share of disposing the asycn command objects carefully
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            _disposalTokenSource.Cancel();
            try
            {
                // --- Block Dispose until all async work has completed.
                ThreadHelper.JoinableTaskFactory.Run(JoinableTaskCollection.JoinTillEmptyAsync);
            }
            catch (OperationCanceledException)
            {
                // --- This exception is expected because we signaled the cancellation token
            }
            catch (AggregateException ex)
            {
                // --- ignore AggregateException containing only OperationCanceledExceptionI
                ex.Handle(inner => inner is OperationCanceledException);
            }
        }

        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected virtual void OnQueryStatus(OleMenuCommand mc)
        {
        }

        /// <summary>
        /// Override this method to execute the command
        /// </summary>
        private void OnExecute()
        {
            IsCancelled = false;
            ExecuteOnMainThread();
            if (IsCancelled)
            {
                return;
            }
            JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    await Task.Yield(); // get off the caller's callstack.
                    await ExecuteAsync();
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    await CompleteOnMainThreadAsync();
                }
                catch (OperationCanceledException)
                {
                    // --- This exception is expected because we signaled the cancellation token
                    IsCancelled = true;
                    OnCancellation();
                }
                catch (AggregateException ex)
                {
                    // --- ignore AggregateException containing only OperationCanceledExceptionI
                    if (ex.InnerException is OperationCanceledException)
                    {
                        IsCancelled = true;
                        OnCancellation();
                    }
                    else
                    {
                        OnException(ex);
                    }
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    await FinallyOnMainThreadAsync();
                    if (UpdateUiWhenComplete)
                    {
                        VsxAsyncPackage.UpdateCommandUi();
                    }
                }
            });
        }

        /// <summary>
        /// Override this method to define how to prepare the command on the
        /// main thread of Visual Studio
        /// </summary>
        protected virtual void ExecuteOnMainThread()
        {
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected virtual Task ExecuteAsync() => Task.FromResult(0);

        /// <summary>
        /// Override this method to define the completion of successful
        /// command execution on the main thread of Visual Studio
        /// </summary>
        protected virtual Task CompleteOnMainThreadAsync() => Task.FromResult(0);

        /// <summary>
        /// Override this command when the async task has been cancelled
        /// </summary>
        protected virtual void OnCancellation()
        {
        }

        /// <summary>
        /// Override this method to handle exceptions
        /// </summary>
        protected virtual void OnException(Exception ex)
        {
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected virtual Task FinallyOnMainThreadAsync() => Task.FromResult(0);
    }
}