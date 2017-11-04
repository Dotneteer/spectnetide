using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class implements a command that can be executed asynchronously
    /// </summary>
    /// <typeparam name="TPackage"></typeparam>
    /// <typeparam name="TCommandSet"></typeparam>
    public abstract class VsxAsyncCommand<TPackage, TCommandSet> : 
        VsxCommand<TPackage, TCommandSet>,
        IDisposable
        where TPackage : VsxPackage 
        where TCommandSet : VsxCommandSet<TPackage>
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
        /// Initialize the async command
        /// </summary>
        protected VsxAsyncCommand()
        {
            JoinableTaskCollection = ThreadHelper.JoinableTaskContext.CreateCollection();
            JoinableTaskFactory = ThreadHelper.JoinableTaskContext.CreateFactory(JoinableTaskCollection);
        }

        /// <inheritdoc />
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
        /// Switches back to the main thread of Visual Studio
        /// </summary>
        protected async Task SwitchToMainThreadAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        }

        /// <summary>
        /// Switches to the background thread
        /// </summary>
        protected async Task SwitchToBackgroundThreadAsync()
        {
            await TaskScheduler.Default;
        }

        /// <summary>
        /// Override this method to execute the command
        /// </summary>
        protected override void OnExecute()
        {
            var cancel = IsCancelled = false;            
            PrepareCommandOnMainThread(ref cancel);
            if (cancel)
            {
                IsCancelled = true;
                return;
            }
            JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    await Task.Yield(); // get off the caller's callstack.
                    await ExecuteAsync();
                    await SwitchToMainThreadAsync();
                    CompleteOnMainThread();
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
                    FinallyOnMainThread();
                }
            });
        }

        /// <summary>
        /// Override this method to define how to prepare the command on the
        /// main thread of Visual Studio
        /// </summary>
        protected virtual void PrepareCommandOnMainThread(ref bool cancel)
        {
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected abstract Task ExecuteAsync();

        /// <summary>
        /// Override this method to define the completion of successful
        /// command execution on the main thread of Visual Studio
        /// </summary>
        protected virtual void CompleteOnMainThread()
        {
        }

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
        protected virtual void FinallyOnMainThread()
        {
        }
    }
}