using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Spect.Net.Z80Emu.Disasm;
using Spect.Net.Z80TestHelpers;

namespace Spect.Net.Z80Tests.ViewModels
{
    public class DisassemblyViewModel: ViewModelBase
    {
        private IReadOnlyList<DisassemblyItem> _disassemblyItems;
        private Z80DisAsmProject _disAsmProject;
        private IList<DisassemblyItem> _selectedItems;

        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public IReadOnlyList<DisassemblyItem> DisassemblyItems
        {
            get { return _disassemblyItems; }
            set { Set(ref _disassemblyItems, value); }
        }

        /// <summary>
        /// The disassembly project belonging to this view model
        /// </summary>
        public Z80DisAsmProject DisAsmProject
        {
            get { return _disAsmProject; }
            set { Set(ref _disAsmProject, value); }
        }

        /// <summary>
        /// Selected disassembly items
        /// </summary>
        public IList<DisassemblyItem> SelectedItems
        {
            get { return _selectedItems; }
            set { Set(ref _selectedItems, value); }
        }

        /// <summary>
        /// Disassemble the current project
        /// </summary>
        public RelayCommand DisassemblyCommand { get; set; }

        /// <summary>
        /// Creates a new label, or updates an existing one
        /// </summary>
        public RelayCommand<DisassemblyLabel> EditLabelCommand { get; set; }

        /// <summary>
        /// Creates a new comment, or updates an existing one
        /// </summary>
        public RelayCommand<DisassemblyComment> EditCommentCommand { get; set; }

        /// <summary>
        /// Creates a disassembly data section
        /// </summary>
        public RelayCommand<DisassemblyDataSection> CreateDataSectionCommand { get; set; }

        /// <summary>
        /// Creates a disassembly data section
        /// </summary>
        public RelayCommand<DisassemblyDataSection> RemoveDataSectionCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyViewModel()
        {
            DisassemblyItems = new List<DisassemblyItem>();
            DisassemblyCommand = new RelayCommand(() =>
            {
                var spectrum48Rom = FileHelper.ExtractResourceFile("ZXSpectrum48.bin");
                var project = new Z80DisAsmProject
                {
                    Z80Binary = spectrum48Rom
                };
                project.SetCustomLabel(0x0018, "GET-CHAR");
                project.SetCustomLabel(0x001C, "TEST-CHAR");
                project.SetCustomComment(0x0008, "Error restart");
                var disasm = new Z80Disassembler(project);
                DisassemblyItems = disasm.Disassemble().OutputItems;
            });
        }
    }
}