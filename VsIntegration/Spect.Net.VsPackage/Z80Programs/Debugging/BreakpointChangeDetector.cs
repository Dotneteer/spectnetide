using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using OutputWindow = Spect.Net.VsPackage.Vsx.Output.OutputWindow;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Z80Programs.Debugging
{
    /// <summary>
    /// This class is responsible for observing Visual Studio breakpoint changes 
    /// </summary>
    public sealed class BreakpointChangeDetector : IDisposable
    {
        private const int IDLE_TIME_IN_MS = 200;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly JoinableTaskFactory _joinableTaskFactory;
        private readonly JoinableTaskCollection _joinableTaskCollection;
        private int _lastBreakpointCount;

        /// <summary>
        /// The host package
        /// </summary>
        public SpectNetPackage Package { get; }

        /// <summary>
        /// Binds this object to the host package
        /// </summary>
        /// <param name="package"></param>
        public BreakpointChangeDetector(SpectNetPackage package)
        {
            Package = package;
            _joinableTaskCollection = ThreadHelper.JoinableTaskContext.CreateCollection();
            _joinableTaskFactory = ThreadHelper.JoinableTaskContext.CreateFactory(_joinableTaskCollection);
            _lastBreakpointCount = -1;
        }

        /// <summary>
        /// Starts wathing for breakpoint changes
        /// </summary>
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _joinableTaskFactory.RunAsync(async () =>
            {
                var pane = OutputWindow.GetPane<Z80OutputPane>();
                pane.WriteLine("BreakpointWatcher started.");

                string exceptionMessage = null;
                try
                {
                    await Task.Yield(); // get off the caller's callstack.
                    await WatchChanges(_cancellationTokenSource.Token);
                    pane.WriteLine("BreakpointWatcher stopped.");

                }
                catch (OperationCanceledException)
                {
                    // --- This exception is expected because we signaled the cancellation token
                }
                catch (AggregateException ex)
                {
                    // --- ignore AggregateException containing only OperationCanceledExceptionI
                    if (!(ex.InnerException is OperationCanceledException))
                    {
                        exceptionMessage = ex.Message;
                    }
                }
                finally
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (exceptionMessage != null)
                    {
                        pane.WriteLine($"BreakpointWatcher exited: {exceptionMessage}");
                    }
                }
            });
        }

        /// <summary>
        /// Stops watching for breakpoint changes
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Implement the procedure to watch for breakpoint changes
        /// </summary>
        /// <param name="token">Cancellation token</param>
        private async Task WatchChanges(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // --- Switch to background thread
                await TaskScheduler.Default;

                // --- Wait a while
                await Task.Delay(IDLE_TIME_IN_MS, token);

                // --- Do the job on the main thread
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                // --- Check for breakpoint number changes
                var breakpoints = Package.ApplicationObject.Debugger.Breakpoints;
                if (breakpoints.Count == _lastBreakpointCount) continue;

                // --- Change detected, let's process them
                _lastBreakpointCount = breakpoints.Count;

                // --- Get the list of active .z80asm files
                var activeAsmFiles = new List<string>();
                foreach (Document doc in Package.ApplicationObject.Documents)
                {
                    if (Path.GetExtension(doc.FullName)?.ToLower() == ".z80asm")
                    {
                        activeAsmFiles.Add(doc.FullName.ToLower());
                    }
                }

                // --- Update the debug layout of the files with breakpoints
                foreach (Breakpoint bp in breakpoints)
                {
                    // --- If the document is active, update it
                    var filename = bp.File.ToLower();
                    if (activeAsmFiles.IndexOf(filename) >= 0)
                    {
                        Package.DebugInfoProvider.UpdateLayoutWithDebugInfo(filename);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Does the lion's share of disposing the asycn command objects carefully
        /// </summary>
        /// <param name="disposing">True, if managed sources should be disposed</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                try
                {
                    // --- Block Dispose until all async work has completed.
                    ThreadHelper.JoinableTaskFactory.Run(_joinableTaskCollection.JoinTillEmptyAsync);
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
        }
    }
}