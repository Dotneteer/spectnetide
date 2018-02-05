using System;
using Spect.Net.VsPackage.Z80Programs;

namespace Spect.Net.VsPackage.ToolWindows.CompilerOutput
{
    /// <summary>
    /// This class represents the view model used in the Z80 Assembler
    /// output tool window
    /// </summary>
    public class AssemblerOutputToolWindowViewModel: SpectNetPackageToolWindowBase
    {
        private bool _hasOutput;
        private DateTime _lastCompilations;
        private bool _errorsDetected;
        private int _errorCount;

        /// <summary>
        /// Indicates if there's any compiler output
        /// </summary>
        public bool HasOutput
        {
            get => _hasOutput;
            set => Set(ref _hasOutput, value);
        }

        /// <summary>
        /// The time of last compilation
        /// </summary>
        public DateTime LastCompilation
        {
            get => _lastCompilations;
            set => Set(ref _lastCompilations, value);
        }

        /// <summary>
        /// Signs that there are errors
        /// </summary>
        public bool ErrorsDetected
        {
            get => _errorsDetected;
            set => Set(ref _errorsDetected, value);
        }

        /// <summary>
        /// Number of errors detected
        /// </summary>
        public int ErrorCount
        {
            get => _errorCount;
            set => Set(ref _errorCount, value);
        }

        /// <summary>
        /// Compilation output
        /// </summary>
        public Assembler.Assembler.AssemblerOutput Output { get; set; }

        public AssemblerOutputToolWindowViewModel()
        {
            if (IsInDesignMode)
            {
                HasOutput = true;
                LastCompilation = DateTime.Now;
                ErrorCount = 5;
                ErrorsDetected = true;
                return;
            }

            Package.CodeManager.CompilationCompleted += OnCompilationCompleted;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Package.CodeManager.CompilationCompleted -= OnCompilationCompleted;
            base.Dispose();
        }

        /// <summary>
        /// Handle the event when a compilation completes
        /// </summary>
        private void OnCompilationCompleted(object sender, CompilationCompletedEventArgs args)
        {
            Output = args.Output;
            HasOutput = true;
            LastCompilation = DateTime.Now;
            ErrorCount = Output.ErrorCount;
            ErrorsDetected = Output.ErrorCount != 0;
            RaisePropertyChanged(nameof(Output));
        }
    }
}