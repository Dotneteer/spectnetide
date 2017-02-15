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

        public IReadOnlyList<DisassemblyItem> DisassemblyItems
        {
            get { return _disassemblyItems; }
            set { Set(ref _disassemblyItems, value); }
        }

        public RelayCommand DisassemblyCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyViewModel()
        {
            DisassemblyItems = new List<DisassemblyItem>();
            DisassemblyCommand = new RelayCommand(() =>
            {
                var spectrum48Rom = FileHelper.ExtractResourceFile("ZXSpectrum48.bin");
                var project = new Z80DisAsmProject(spectrum48Rom);
                project.LabelStore.SetCustomLabel(0x0018, "GET-CHAR");
                project.LabelStore.SetCustomLabel(0x001C, "TEST-CHAR");
                var disasm = new Z80Disassembler(project);
                DisassemblyItems = disasm.Disassemble().OutputItems;
            });
        }
    }
}