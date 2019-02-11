namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This view model is the base of the view models that manage memory
    /// </summary>
    public class BankAwareToolWindowViewModelBase : SpectrumGenericToolWindowViewModel
    {
        private bool _fullViewMode;
        private bool _romViewMode;
        private bool _ramBankViewMode;
        private int _romIndex;
        private int _ramBankIndex;

        /// <summary>
        /// The full 64K memory is displayed
        /// </summary>
        public bool FullViewMode
        {
            get => _fullViewMode;
            set
            {
                if (!Set(ref _fullViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// A ROM page is displayed
        /// </summary>
        public bool RomViewMode
        {
            get => _romViewMode;
            set
            {
                if (!Set(ref _romViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// A RAM bank page is displayed
        /// </summary>
        public bool RamBankViewMode
        {
            get => _ramBankViewMode;
            set
            {
                if (!Set(ref _ramBankViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// The index of the ROM page displayed in ROM view mode
        /// </summary>
        public int RomIndex
        {
            get => _romIndex;
            set => Set(ref _romIndex, value);
        }

        /// <summary>
        /// The index of the RAM page displayed in RAM bank view mode
        /// </summary>
        public int RamBankIndex
        {
            get => _ramBankIndex;
            set => Set(ref _ramBankIndex, value);
        }

        /// <summary>
        /// Should the ROM tag be displayed?
        /// </summary>
        public bool ShowRomTag => FullViewMode || RomViewMode;

        /// <summary>
        /// Should the BANK tag be displayed?
        /// </summary>
        public bool ShowBankTag => FullViewMode || RamBankViewMode;

        /// <summary>
        /// This flag indicates if the Bank view is allowed or not
        /// </summary>
        public bool BankViewAllowed => (MachineViewModel?.SpectrumVm
                                            ?.RomConfiguration?.NumberOfRoms ?? 0) > 1;
        /// <summary>
        /// Retrieves the memory buffer to show
        /// </summary>
        protected byte[] GetMemoryBuffer()
        {
            return FullViewMode
                ? MachineViewModel?.SpectrumVm?.MemoryDevice?.CloneMemory()
                : (RomViewMode
                    ? MachineViewModel?.SpectrumVm?.MemoryDevice?.GetRomBuffer(RomIndex)
                    : MachineViewModel?.SpectrumVm?.MemoryDevice?.GetRamBank(RamBankIndex));
        }

        /// <summary>
        /// Retrieves the length of the memory buffer to show
        /// </summary>
        protected int? GetMemoryLength()
        {
            return FullViewMode
                ? 0x10000
                : 0x4000;
        }

        /// <summary>
        /// Sets the current view to full view mode
        /// </summary>
        public void SetFullViewMode()
        {
            FullViewMode = true;
            RomViewMode = false;
            RamBankViewMode = false;
            UpdatePageInformation();
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to ROM view mode
        /// </summary>
        /// <param name="romIndex">The ROM page to show</param>
        public void SetRomViewMode(int romIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RomIndex = romIndex;
            RomViewMode = true;
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to RAM bank view mode
        /// </summary>
        /// <param name="ramBankIndex">The RAM bank to show</param>
        public void SetRamBankViewMode(int ramBankIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RamBankIndex = ramBankIndex;
            RamBankViewMode = true;
            InitViewMode();
        }

        /// <summary>
        /// Updates the page information
        /// </summary>
        public void UpdatePageInformation()
        {
            RomIndex = MachineViewModel?.SpectrumVm?.MemoryDevice?.GetSelectedRomIndex() ?? 0;
            RamBankIndex = MachineViewModel?.SpectrumVm?.MemoryDevice?.GetSelectedBankIndex(3) ?? 0;
        }

        /// <summary>
        /// When the view model is first time created, use the ROM view
        /// </summary>
        protected override void Initialize()
        {
            RaisePropertyChanged(nameof(BankViewAllowed));
            SetRomViewMode(0);
        }

        /// <summary>
        /// Set the tool window into Full view mode on first start
        /// </summary>
        protected override void OnFirstStart()
        {
            SetFullViewMode();
        }

        /// <summary>
        /// Refresh the view mode for every start/continue
        /// </summary>
        protected override void OnStart()
        {
            RefreshViewMode();
        }

        /// <summary>
        /// Refresh the memory view for each pause
        /// </summary>
        protected override void OnPaused()
        {
            if (FullViewMode)
            {
                UpdatePageInformation();
            }
            RefreshOnPause();
        }

        /// <summary>
        /// Override to handle the first paused state
        /// of the virtual machine
        /// </summary>
        protected override void OnFirstPaused()
        {
            SetFullViewMode();
        }

        /// <summary>
        /// Set the ROM view mode when the machine is stopped
        /// </summary>
        protected override void OnStopped()
        {
            SetRomViewMode(0);
        }

        /// <summary>
        /// Override this method to init the current view mode
        /// </summary>
        public virtual void InitViewMode()
        {
        }

        /// <summary>
        /// Override this method to refresh the current view mode
        /// </summary>
        public virtual void RefreshViewMode()
        {
        }

        /// <summary>
        /// Override this method to define how to refresh the view 
        /// when the virtual machine is paused
        /// </summary>
        public virtual void RefreshOnPause()
        {
        }

        /// <summary>
        /// Refreshes the item at the current address
        /// </summary>
        /// <param name="addr">Item address</param>
        public virtual void RefreshItem(int addr)
        {
        }

        /// <summary>
        /// Refreshes the item at the specified address, if it 
        /// should be refreshed in the current view
        /// </summary>
        /// <param name="addr"></param>
        public virtual void RefreshItemIfItMayChange(ushort addr)
        {
            // --- ROM items never change
            if (RomViewMode) return;

            if (RamBankViewMode)
            {
                var memDevice = MachineViewModel.SpectrumVm.MemoryDevice;
                if (!memDevice.IsRamBankPagedIn(RamBankIndex, out _))
                {
                    // --- The current RAM bank is not paged in, so it may not change
                    return;
                }
            }

            // --- The content at the address may change
            RefreshItem(addr);
        }
    }
}