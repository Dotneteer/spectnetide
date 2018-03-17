using Spect.Net.VsPackage.Z80Programs;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This view model is the base of the view models that manage memory
    /// </summary>
    public class BankAwareToolWindowViewModelBase : SpectrumGenericToolWindowViewModel
    {
        private bool _bankViewAllowed;
        private bool _fullViewMode;
        private bool _romViewMode;
        private bool _ramBankViewMode;
        private bool _ramPageViewMode;
        private bool _divIdeBankViewMode;
        private bool _show8KMode;
        private bool _isDivIdePagedIn;
        private int _romIndex;
        private int _ramBankIndex;
        private int _ramPageIndex;
        private int _divIdeBankIndex;
        private string _allRamConfig;
        private string _divIdeRamMapFlag;
        private bool _showLocked;
        private bool _showAllRamTag;

        #region Properties

        /// <summary>
        /// This flag indicates if the Bank view is allowed or not
        /// </summary>
        public bool BankViewAllowed
        {
            get => _bankViewAllowed;
            set => Set(ref _bankViewAllowed, value);
        }

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
                RaisePropertyChanged(nameof(ShowPageTag));
                RaisePropertyChanged(nameof(ShowDivIdeTag));
                RaisePropertyChanged(nameof(Show8KMode));
                RaisePropertyChanged(nameof(ShowAllRamTag));
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
                RaisePropertyChanged(nameof(ShowPageTag));
                RaisePropertyChanged(nameof(ShowDivIdeTag));
                RaisePropertyChanged(nameof(Show8KMode));
                RaisePropertyChanged(nameof(ShowAllRamTag));
            }
        }

        /// <summary>
        /// A 16K RAM bank is displayed
        /// </summary>
        public bool RamBankViewMode
        {
            get => _ramBankViewMode;
            set
            {
                if (!Set(ref _ramBankViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
                RaisePropertyChanged(nameof(ShowPageTag));
                RaisePropertyChanged(nameof(ShowDivIdeTag));
                RaisePropertyChanged(nameof(Show8KMode));
                RaisePropertyChanged(nameof(ShowAllRamTag));
            }
        }

        /// <summary>
        /// A 8K RAM page is displayed
        /// </summary>
        public bool RamPageViewMode
        {
            get => _ramPageViewMode;
            set
            {
                if (!Set(ref _ramPageViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
                RaisePropertyChanged(nameof(ShowPageTag));
                RaisePropertyChanged(nameof(ShowDivIdeTag));
                RaisePropertyChanged(nameof(Show8KMode));
                RaisePropertyChanged(nameof(ShowAllRamTag));
            }
        }

        /// <summary>
        /// A 8K DivIDE page is displayed
        /// </summary>
        public bool DivIdeBankViewMode
        {
            get => _divIdeBankViewMode;
            set
            {
                if (!Set(ref _divIdeBankViewMode, value)) return;

                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
                RaisePropertyChanged(nameof(ShowPageTag));
                RaisePropertyChanged(nameof(ShowDivIdeTag));
                RaisePropertyChanged(nameof(Show8KMode));
                RaisePropertyChanged(nameof(ShowAllRamTag));
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
        /// The index of the RAM bank displayed in RAM bank mode
        /// </summary>
        public int RamBankIndex
        {
            get => _ramBankIndex;
            set => Set(ref _ramBankIndex, value);
        }

        /// <summary>
        /// The index of RAM page displayes in RAM page mode
        /// </summary>
        public int RamPageIndex
        {
            get => _ramPageIndex;
            set => Set(ref _ramPageIndex, value);
        }

        /// <summary>
        /// The index of DivIDE bank index
        /// </summary>
        public int DivIdeBankIndex
        {
            get => _divIdeBankIndex;
            set => Set(ref _divIdeBankIndex, value);
        }

        /// <summary>
        /// Contains the ALLRAM page configurations
        /// </summary>
        public string AllRamConfig
        {
            get => _allRamConfig;
            set => Set(ref _allRamConfig, value);
        }

        /// <summary>
        /// Contains the DivIDE RAMMAP flag
        /// </summary>
        public string DivIdeRamMapFlag
        {
            get => _divIdeRamMapFlag;
            set => Set(ref _divIdeRamMapFlag, value);
        }

        /// <summary>
        /// Should the ROM tag be displayed?
        /// </summary>
        public bool ShowRomTag => RomViewMode || FullViewMode && !ShowAllRamTag;

        /// <summary>
        /// Should the BANK tag be displayed?
        /// </summary>
        public bool ShowBankTag => RamBankViewMode || FullViewMode && !Show8KMode && !ShowAllRamTag && !ShowLocked;

        /// <summary>
        /// Should the PAGE tag be displayed?
        /// </summary>
        public bool ShowPageTag => RamPageViewMode || FullViewMode && Show8KMode && !ShowAllRamTag;

        /// <summary>
        /// Should the 8K tag displayed?
        /// </summary>
        public bool Show8KMode
        {
            get => _show8KMode;
            set => Set(ref _show8KMode, value);
        }

        /// <summary>
        /// Should the DIVIDE tag be displayed?
        /// </summary>
        public bool ShowDivIdeTag => DivIdeBankViewMode || FullViewMode && _isDivIdePagedIn && !ShowAllRamTag;

        /// <summary>
        /// Should the ALLRAM tag be displayed?
        /// </summary>
        public bool ShowAllRamTag
        {
            get => _showAllRamTag;
            set
            {
                if (!Set(ref _showAllRamTag, value)) return;
                RaisePropertyChanged(nameof(ShowRomTag));
                RaisePropertyChanged(nameof(ShowBankTag));
                RaisePropertyChanged(nameof(ShowPageTag));
                RaisePropertyChanged(nameof(ShowDivIdeTag));
                RaisePropertyChanged(nameof(Show8KMode));
            }
        }

        /// <summary>
        /// Should the Paging locked tag be displayed?
        /// </summary>
        public bool ShowLocked
        {
            get => _showLocked;
            set
            {
                if (!Set(ref _showLocked, value)) return;
                RaisePropertyChanged(nameof(ShowBankTag));
            }
        }

        /// <summary>
        /// Compiler output
        /// </summary>
        public Assembler.Assembler.AssemblerOutput CompilerOutput { get; private set; }

        #endregion

        #region Lifecycle methods

        /// <summary>
        /// Initializes the view model
        /// </summary>
        public BankAwareToolWindowViewModelBase()
        {
            if (IsInDesignMode)
            {
                BankViewAllowed = true;
                ShowAllRamTag = false;
                _isDivIdePagedIn = true;
                Show8KMode = true;
                FullViewMode = true;
                RomViewMode = false;
                RamBankViewMode = false;
                RamPageViewMode = false;
                DivIdeBankViewMode = false;
                RomIndex = 1;
                RamBankIndex = 2;
                RamPageIndex = 12;
                DivIdeRamMapFlag = "W";
                DivIdeBankIndex = 3;
                AllRamConfig = "3456";
                ShowLocked = true;
                return;
            }
            BankViewAllowed = (MachineViewModel?.SpectrumVm
                                   ?.RomConfiguration?.NumberOfRoms ?? 0) > 1;
        }

        /// <summary>
        /// Override this method to handle the solution opened event
        /// </summary>
        protected override void OnSolutionOpened()
        {
            base.OnSolutionOpened();
            BankViewAllowed = (MachineViewModel?.SpectrumVm
                                   ?.RomConfiguration?.NumberOfRoms ?? 0) > 1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Package.CodeManager.CompilationCompleted -= OnCompilationCompleted;
            base.Dispose();
        }

        #endregion

        /// <summary>
        /// Catch the event of compilation
        /// </summary>
        private void OnCompilationCompleted(object sender, CompilationCompletedEventArgs e)
        {
            CompilerOutput = e.Output;
        }

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
            RamPageViewMode = false;
            DivIdeBankViewMode = false;
            ShowAllRamTag = false;
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
            RomViewMode = true;
            RamBankViewMode = false;
            RamPageViewMode = false;
            DivIdeBankViewMode = false;
            RomIndex = romIndex;
            ShowAllRamTag = false;
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to 16K RAM bank view mode
        /// </summary>
        /// <param name="ramBankIndex">The RAM bank to show</param>
        public void SetRamBankViewMode(int ramBankIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = true;
            RamPageViewMode = false;
            DivIdeBankViewMode = false;
            RamBankIndex = ramBankIndex;
            ShowAllRamTag = false;
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to 8K RAM page view mode
        /// </summary>
        /// <param name="ramPageIndex">The RAM page to show</param>
        public void SetRamPageViewMode(int ramPageIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RamPageViewMode = true;
            DivIdeBankViewMode = false;
            RamBankIndex = ramPageIndex;
            ShowAllRamTag = false;
            InitViewMode();
        }

        /// <summary>
        /// Sets the current view to 8K DivIDE bank view mode
        /// </summary>
        /// <param name="divIdeBankIndex">The DivIde bank to show</param>
        public void SetDivIdeBankViewMode(int divIdeBankIndex)
        {
            FullViewMode = false;
            RomViewMode = false;
            RamBankViewMode = false;
            RamPageViewMode = false;
            DivIdeBankViewMode = true;
            DivIdeBankIndex = divIdeBankIndex;
            ShowAllRamTag = false;
            InitViewMode();
        }

        /// <summary>
        /// Updates the page information
        /// </summary>
        public void UpdatePageInformation()
        {
            var memDevice = MachineViewModel?.SpectrumVm?.MemoryDevice;
            if (memDevice != null)
            {
                ShowLocked = !memDevice.PagingEnabled;
                RomIndex = memDevice.GetSelectedRomIndex();
                RamBankIndex = memDevice.GetSelectedBankIndex(3);
                ShowAllRamTag = memDevice.IsInAllRamMode;
                Show8KMode = memDevice.IsIn8KMode && !ShowAllRamTag;
                if (ShowAllRamTag)
                {
                    var allRam = "";
                    for (var i = 0; i < 4; i++)
                    {
                        allRam += memDevice.GetSelectedBankIndex(i);
                    }
                    AllRamConfig = allRam;
                }
            }

            var divIdeDevice = MachineViewModel?.SpectrumVm?.DivIdeDevice;
            if (divIdeDevice != null)
            {
                _isDivIdePagedIn = divIdeDevice.IsDivIdePagedIn;
                DivIdeBankIndex = divIdeDevice.Bank;
                DivIdeRamMapFlag = divIdeDevice.MapRam ? "" : "W";
            }
            else
            {
                _isDivIdePagedIn = false;
            }
        }

        /// <summary>
        /// When the view model is first time created, use the ROM view
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            Package.CodeManager.CompilationCompleted += OnCompilationCompleted;
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