using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This view model represents the disassembly list
    /// </summary>
    public class DisassemblyViewModel: EnhancedViewModelBase
    {
        private ObservableCollection<DisassemblyItemViewModel> _disassemblyItems;

        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public ObservableCollection<DisassemblyItemViewModel> DisassemblyItems
        {
            get => _disassemblyItems;
            private set => Set(ref _disassemblyItems, value);
        }

        /// <summary>
        /// Gets the line index
        /// </summary>
        public Dictionary<ushort, int> LineIndexes { get; private set; }

        /// <summary>
        /// Stores the object that handles annotations and their persistence
        /// </summary>
        public DisassemblyAnnotationHandler AnnotationHandler { get; private set; }

        public DisassemblyViewModel()
        {
            if (IsInDesignMode)
            {
                DisassemblyItems = new ObservableCollection<DisassemblyItemViewModel>
                {
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel(),
                    new DisassemblyItemViewModel()
                };
                return;
            }

            InitDisassembly();
        }

        /// <summary>
        /// Initializes disassembly items and annotations
        /// </summary>
        protected virtual void InitDisassembly()
        {
        }
    }
}