using System.Collections.Generic;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

#pragma warning disable VSTHRD010

namespace Spect.Net.VsPackage.Debugging
{
    /// <summary>
    /// This class checks if a breakpoint has been changed
    /// </summary>
    public class BreakpointChangeWatcher
    {
        /// <summary>
        /// Check period waiting time in milleseconds
        /// </summary>
        public const int WATCH_PERIOD = 100;

        private CancellationTokenSource _cancellationSource;
        private JoinableTask _changeWatcherTask;

        /// <summary>
        /// Starts watching breakpoint changes
        /// </summary>
        public void Start()
        {
            if (_changeWatcherTask != null) return;
            _cancellationSource = new CancellationTokenSource();
            _changeWatcherTask = SpectNetPackage.Default.JoinableTaskFactory.StartOnIdle(
                () => CheckBreakpointChangesAsync(_cancellationSource.Token), 
                VsTaskRunContext.UIThreadIdlePriority);
        }

        /// <summary>
        /// Stops watching breakpoint changes
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            _cancellationSource.Cancel();
            await _changeWatcherTask;
            _cancellationSource = null;
            _changeWatcherTask = null;
        }

        /// <summary>
        /// Signs that the collection of breakpoints changed.
        /// </summary>
        public event AsyncEventHandler<BreakpointsChangedEventArgs> BreakpointsChanged;

        /// <summary>
        /// Checks if any of the breakpoints has been changed.
        /// </summary>
        /// <param name="cancellationToken">Cancels running this task</param>
        /// <remark>
        /// This task always runs on the main thread in idle time.
        /// </remark>
        private async Task CheckBreakpointChangesAsync(CancellationToken cancellationToken)
        {
            // --- Store the original breakpoint count
            var prevCount = 0;
            var loopCount = 0;
            var prevBreakpoints = new Dictionary<Breakpoint, BreakpointWithState>();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(WATCH_PERIOD, cancellationToken);
                loopCount++;
                var currentCount = SpectNetPackage.Default.ApplicationObject.DTE.Debugger.Breakpoints?.Count ?? 0;

                // --- We examine new and deleted breakpoints immediately, changed breakpoints only
                // --- occasionally
                if (currentCount == prevCount && loopCount % 5 != 0) continue;

                // --- Collect changes
                var added = new List<Breakpoint>();
                var modified = new List<Breakpoint>();
                var deleted = new List<Breakpoint>();
                var vsBreakpoints = SpectNetPackage.Default.ApplicationObject.DTE.Debugger.Breakpoints;
                if (vsBreakpoints != null)
                {
                    // --- Collect modified breakpoints
                    var tempBreakpoints = new Dictionary<Breakpoint, BreakpointWithState>();
                    foreach (Breakpoint currentBreakpoint in vsBreakpoints)
                    {
                        tempBreakpoints.Add(currentBreakpoint, new BreakpointWithState
                        {
                            Breakpoint = currentBreakpoint,
                            Enabled = currentBreakpoint.Enabled,
                            Condition = currentBreakpoint.Condition,
                            ConditionType = currentBreakpoint.ConditionType,
                            HitCountType = currentBreakpoint.HitCountType,
                            HitCountTarget = currentBreakpoint.HitCountTarget
                        });
                        if (!prevBreakpoints.TryGetValue(currentBreakpoint, out var oldBreak))
                        {
                            added.Add(currentBreakpoint);
                        }
                        else
                        {
                            if (oldBreak.Enabled != currentBreakpoint.Enabled
                                || oldBreak.Condition != currentBreakpoint.Condition
                                || oldBreak.ConditionType != currentBreakpoint.ConditionType
                                || oldBreak.HitCountType != currentBreakpoint.HitCountType
                                || oldBreak.HitCountTarget != currentBreakpoint.HitCountTarget)
                            {
                                modified.Add(currentBreakpoint);
                            }
                        }
                    }

                    // --- Collect deleted breakpoints
                    foreach (var currentBreakpoint in prevBreakpoints.Values)
                    {
                        if (!tempBreakpoints.ContainsKey(currentBreakpoint.Breakpoint))
                        {
                            deleted.Add(currentBreakpoint.Breakpoint);
                        }
                    }
                    prevBreakpoints = tempBreakpoints;
                }

                prevCount = currentCount;
                if (added.Count == 0 && modified.Count == 0 && deleted.Count == 0) continue;

                if (BreakpointsChanged != null)
                {
                    await BreakpointsChanged.InvokeAsync(this, new BreakpointsChangedEventArgs(added, modified, deleted));
                }
            }
        }

        /// <summary>
        /// This structure represents a breakpoint with its previous state
        /// </summary>
        private struct BreakpointWithState
        {
            public Breakpoint Breakpoint;
            public bool Enabled;
            public string Condition;
            public dbgBreakpointConditionType ConditionType;
            public dbgHitCountType HitCountType;
            public int HitCountTarget;
        }
    }
}
