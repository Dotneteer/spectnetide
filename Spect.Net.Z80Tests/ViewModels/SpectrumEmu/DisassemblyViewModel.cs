﻿using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Spect.Net.Z80Emu.Disasm;
using Spect.Net.Z80Tests.Mvvm.Navigation;
using Spect.Net.Z80Tests.SpectrumHost;

namespace Spect.Net.Z80Tests.ViewModels.SpectrumEmu
{
    public class DisassemblyViewModel: ViewModelBaseWithDesignTimeFix
    {
        private IReadOnlyList<DisassemblyItemViewModel> _disassemblyItems;
        private Z80DisassembyProject _disassembyProject;

        /// <summary>
        /// The disassembly items belonging to this project
        /// </summary>
        public IReadOnlyList<DisassemblyItemViewModel> DisassemblyItems
        {
            get { return _disassemblyItems; }
            set { Set(ref _disassemblyItems, value); }
        }

        /// <summary>
        /// The disassembly project belonging to this view model
        /// </summary>
        public Z80DisassembyProject DisassembyProject
        {
            get { return _disassembyProject; }
            set { Set(ref _disassembyProject, value); }
        }

        /// <summary>
        /// The Debug view model of this virtual machine
        /// </summary>
        public SpectrumDebugViewModel DebugViewModel { get; }

        /// <summary>
        /// Toggles the breakpoint associated with the selected item
        /// </summary>
        public RelayCommand ToggleBreakpointCommand { get; set; }

        /// <summary>
        /// Selected disassembly items
        /// </summary>
        public IList<DisassemblyItemViewModel> SelectedItems => _disassemblyItems.Where(item => item.IsSelected).ToList();

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public DisassemblyViewModel()
        {
            DisassemblyItems = new List<DisassemblyItemViewModel>();
        }

        /// <summary>
        /// Sets up this view model instance
        /// </summary>
        public DisassemblyViewModel(SpectrumDebugViewModel debugVm): this()
        {
            DebugViewModel = debugVm;
            ToggleBreakpointCommand = new RelayCommand(OnToggleBreakpoint);
        }

        /// <summary>
        /// Disassembles the current virtual machine's memory
        /// </summary>
        public void Disassemble()
        {
            if (DebugViewModel.SpectrumVm == null) return;
            var spectrum48Rom = new ResourceRomProvider().LoadRom("ZXSpectrum48.rom");
            var project = new Z80DisassembyProject();
            project.SetZ80Binary(spectrum48Rom);
            project.SetCustomLabel(0x0008, "ERROR_RESTART");
            project.SetCustomLabel(0x0018, "GET_CHAR");
            project.SetCustomLabel(0x001C, "TEST_CHAR");
            project.SetComment(0x0008, "Error restart");
            project.AddDataSection(new DisassemblyDataSection(0x0013, 0x0016, DataSectionType.Word));
            project.AddDataSection(new DisassemblyDataSection(0x0017, 0x0017, DataSectionType.Byte));
            project.Disassemble();
            DisassemblyItems = project.Output.OutputItems
                .Select(di => new DisassemblyItemViewModel(di, DebugViewModel))
                .ToList();
        }

        private void OnToggleBreakpoint()
        {
            var items = SelectedItems;
            foreach (var selItem in items)
            {
                var address = selItem.Item.Address;
                var breakpoints = DebugViewModel.DebugInfoProvider.Breakpoints;
                if (breakpoints.Contains(address))
                {
                    breakpoints.Remove(address);
                }
                else
                {
                    breakpoints.Add(address);
                }
                selItem.RaisePropertyChanged(nameof(selItem.HasBreakpoint));
            }
        }
    }
}