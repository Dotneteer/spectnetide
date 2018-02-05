using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.Assembler.Assembler;
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
        private string _toggleSymbolCommandText;
        private List<AssemblySymbol> _symbols;
        private string _toggleFixupCommandText;
        private ObservableCollection<FixupInfo> _fixups;

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

        /// <summary>
        /// Command that toggles symbol orders
        /// </summary>
        public RelayCommand ToggleSymbolOrderCommand { get; set; }

        /// <summary>
        /// Indicates that symbols are ordered by name
        /// </summary>
        public bool OrderBySymbolNames { get; set; }

        /// <summary>
        /// Text of the symbol toggle button
        /// </summary>
        public string ToggleSymbolCommandText
        {
            get => _toggleSymbolCommandText;
            set => Set(ref _toggleSymbolCommandText, value);
        }

        /// <summary>
        /// Symbols to display
        /// </summary>
        public List<AssemblySymbol> Symbols
        {
            get => _symbols;
            set => Set(ref _symbols, value);
        }

        /// <summary>
        /// Command that toggles fixup order
        /// </summary>
        public RelayCommand ToggleFixupFilterCommand { get; set; }

        /// <summary>
        /// Shows that only unresolved fixups should be displayed
        /// </summary>
        public bool FilterToUnresolvedFixups { get; set; }

        /// <summary>
        /// Text of the fixup toggle button
        /// </summary>
        public string ToggleFixupCommandText
        {
            get => _toggleFixupCommandText;
            set => Set(ref _toggleFixupCommandText, value);
        }

        /// <summary>
        /// Fixups to display
        /// </summary>
        public ObservableCollection<FixupInfo> Fixups
        {
            get => _fixups;
            set => Set(ref _fixups, value);
        }

        public AssemblerOutputToolWindowViewModel()
        {
            if (IsInDesignMode)
            {
                HasOutput = true;
                LastCompilation = DateTime.Now;
                ErrorCount = 5;
                ErrorsDetected = true;
                ToggleSymbolCommandText = "Order";
                return;
            }

            Symbols = new List<AssemblySymbol>();
            Package.CodeManager.CompilationCompleted += OnCompilationCompleted;
            ToggleSymbolOrderCommand = new RelayCommand(OnToggleSymbolOrder);
            ToggleFixupFilterCommand = new RelayCommand(OnToggleFixupFilter);

            OrderBySymbolNames = false;
            ToggleSymbolOrderCommand.Execute(null);
            FilterToUnresolvedFixups = false;
            ToggleFixupFilterCommand.Execute(null);
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
            RefreshSymbols();
            RefreshFixups();
        }

        /// <summary>
        /// Executes the Toggle Symbol Order command
        /// </summary>
        private void OnToggleSymbolOrder()
        {
            OrderBySymbolNames = !OrderBySymbolNames;
            RefreshSymbols();
        }

        /// <summary>
        /// Refreshes symbols in the view
        /// </summary>
        private void RefreshSymbols()
        {
            if (OrderBySymbolNames)
            {
                ToggleSymbolCommandText = "Order by values";
                if (Output == null) return;
                var symbols = Output.Symbols.OrderBy(kv => kv.Key).Select(kv => new AssemblySymbol(kv.Key, kv.Value));
                Symbols = new List<AssemblySymbol>(symbols);
            }
            else
            {
                ToggleSymbolCommandText = "Order by symbols";
                if (Output == null) return;
                var symbols = Output.Symbols.OrderBy(kv => kv.Value).Select(kv => new AssemblySymbol(kv.Key, kv.Value));
                Symbols = new List<AssemblySymbol>(symbols);
            }
        }

        /// <summary>
        /// Executes the Toggle Fixup Filter command
        /// </summary>
        private void OnToggleFixupFilter()
        {
            FilterToUnresolvedFixups = !FilterToUnresolvedFixups;
            RefreshFixups();
        }

        /// <summary>
        /// Refreshes fixups in the view
        /// </summary>
        private void RefreshFixups()
        {
            ToggleFixupCommandText = FilterToUnresolvedFixups
                ? "Show all fixups"
                : "Show unresolved fixups";
            if (Output == null) return;

            IEnumerable<FixupEntry> fixups = Output.Fixups;
            if (FilterToUnresolvedFixups)
            {
                fixups = Output.Fixups.Where(fu => !fu.Resolved);
            }

            var fixupInfo = fixups.OrderBy(fu => fu.SegmentIndex)
                .ThenBy(fu => fu.Offset)
                .Select(fu => new FixupInfo(fu.Type, 
                    fu.SegmentIndex, 
                    (ushort)fu.Offset, 
                    fu.Resolved, 
                    fu.Type == FixupType.Equ
                        ? fu.Label
                        : fu.Expression.SourceText
                    ));
            Fixups = new ObservableCollection<FixupInfo>(fixupInfo);
        }
    }
}