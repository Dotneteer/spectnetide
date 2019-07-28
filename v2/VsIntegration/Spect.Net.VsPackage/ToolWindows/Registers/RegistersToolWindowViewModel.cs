using Spect.Net.SpectrumEmu.Machine;

// ReSharper disable InconsistentNaming

namespace Spect.Net.VsPackage.ToolWindows.Registers
{
    public class RegistersToolWindowViewModel: SpectrumGenericToolWindowViewModel
    {
        private ushort _af;
        private byte _f;
        private ushort _bc;
        private ushort _de;
        private ushort _hl;
        private ushort _af_;
        private ushort _bc_;
        private ushort _de_;
        private ushort _hl_;
        private ushort _pc;
        private ushort _sp;
        private ushort _ix;
        private ushort _iy;
        private ushort _ir;
        private ushort _mw;

        private int _im;
        private int _iff1;
        private int _iff2;
        private int _halted;

        private long _tacts;

        private int _frameCount;
        private string _currentUlaTact;
        private string _currentRasterLine;
        private string _currentRasterPos;
        private string _pixelOp;
        private long _lastStepTacts;
        private string _contentionValue;
        private long _contentionAccumulated;
        private long _lastContentionValue;

        public ushort AF
        {
            get => _af;
            set => Set(ref _af, value);
        }

        public byte F
        {
            get => _f;
            set => Set(ref _f, value);
        }

        public ushort BC
        {
            get => _bc;
            set => Set(ref _bc, value);
        }

        public ushort DE
        {
            get => _de;
            set => Set(ref _de, value);
        }

        public ushort HL
        {
            get => _hl;
            set => Set(ref _hl, value);
        }

        public ushort _AF_
        {
            get => _af_;
            set => Set(ref _af_, value);
        }

        public ushort _BC_
        {
            get => _bc_;
            set => Set(ref _bc_, value);
        }

        public ushort _DE_
        {
            get => _de_;
            set => Set(ref _de_, value);
        }

        public ushort _HL_
        {
            get => _hl_;
            set => Set(ref _hl_, value);
        }

        public ushort PC
        {
            get => _pc;
            set => Set(ref _pc, value);
        }

        public ushort SP
        {
            get => _sp;
            set => Set(ref _sp, value);
        }

        public ushort IX
        {
            get => _ix;
            set => Set(ref _ix, value);
        }

        public ushort IY
        {
            get => _iy;
            set => Set(ref _iy, value);
        }

        public ushort IR
        {
            get => _ir;
            set => Set(ref _ir, value);
        }

        public ushort MW
        {
            get => _mw;
            set => Set(ref _mw, value);
        }

        public int IM
        {
            get => _im;
            set => Set(ref _im, value);
        }

        public int IFF1
        {
            get => _iff1;
            set => Set(ref _iff1, value);
        }

        public int IFF2
        {
            get => _iff2;
            set => Set(ref _iff2, value);
        }

        public int Halted
        {
            get => _halted;
            set => Set(ref _halted, value);
        }

        public long Tacts
        {
            get => _tacts;
            set => Set(ref _tacts, value);
        }

        public int FrameCount
        {
            get => _frameCount;
            set => Set(ref _frameCount, value);
        }

        public string CurrentUlaTact
        {
            get => _currentUlaTact;
            set => Set(ref _currentUlaTact, value);
        }

        public string CurrentRasterLine
        {
            get => _currentRasterLine;
            set => Set(ref _currentRasterLine, value);
        }

        public string CurrentRasterPos
        {
            get => _currentRasterPos;
            set => Set(ref _currentRasterPos, value);
        }

        public string PixelOperation
        {
            get => _pixelOp;
            set => Set(ref _pixelOp, value);
        }

        public string ContentionValue
        {
            get => _contentionValue;
            set => Set(ref _contentionValue, value);
        }

        public long LastStepTacts
        {
            get => _lastStepTacts;
            set => Set(ref _lastStepTacts, value);
        }

        public long ContentionAccumulated
        {
            get => _contentionAccumulated;
            set => Set(ref _contentionAccumulated, value);
        }

        public long LastContentionValue
        {
            get => _lastContentionValue;
            set => Set(ref _lastContentionValue, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public RegistersToolWindowViewModel()
        {
            ResetRegisters();
        }

        /// <summary>
        /// Refreshes the Registers view
        /// </summary>
        public void Refresh()
        {
            var state = MachineState;
            if (state == VmState.Running || state == VmState.Paused)
            {
                BindTo();
            }
        }

        /// <summary>
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(VmState oldState, VmState newState)
        {
            Refresh();
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected override void OnScreenRefreshed()
        {
            if (ScreenRefreshCount % 5 == 0)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Bind these registers to the Z80 CPU's register values
        /// </summary>
        public void BindTo()
        {
            var cpu = EmulatorViewModel?.Machine?.Cpu;
            if (cpu == null) return;

            AF = cpu.AF;
            F = cpu.F;
            BC = cpu.BC;
            DE = cpu.DE;
            HL = cpu.HL;
            _AF_ = cpu._AF_;
            _BC_ = cpu._BC_;
            _DE_ = cpu._DE_;
            _HL_ = cpu._HL_;
            PC = cpu.PC;
            SP = cpu.SP;
            IX = cpu.IX;
            IY = cpu.IY;
            IR = cpu.IR;
            MW = cpu.WZ;

            // --- Other CPU state
            IM = cpu.InterruptMode;
            IFF1 = cpu.IFF1 ? 1 : 0;
            IFF2 = cpu.IFF2 ? 1 : 0;
            Halted = cpu.Halted ? 1 : 0;
            Tacts = cpu.Tacts;

            // --- ULA screen inf
            var paused = MachineState == VmState.Paused;

            var ulaTacts = Machine.ScreenConfiguration.ScreenRenderingFrameTactCount;
            if (paused)
            {
                var currentTact = Machine.CurrentFrameTact % ulaTacts;
                CurrentUlaTact = $"{currentTact}";
                CurrentRasterLine = $"{currentTact / Machine.ScreenConfiguration.ScreenLineTime}";
                CurrentRasterPos = $"{currentTact % Machine.ScreenConfiguration.ScreenLineTime}";
                var rt = Machine.ScreenRenderingTable[currentTact];
                PixelOperation = rt.Phase.ToString();
                ContentionValue = rt.ContentionDelay.ToString();
            }
            else
            {
                CurrentUlaTact = "---";
                CurrentRasterLine = "---";
                CurrentRasterPos = "---";
                PixelOperation = "---";
                ContentionValue = "---";
            }

            // --- Step information
            LastStepTacts = cpu.Tacts - Machine.LastExecutionStartTact;
            ContentionAccumulated = Machine.ContentionAccumulated;
            LastContentionValue = Machine.ContentionAccumulated - Machine.LastExecutionContentionValue;
        }

        private void ResetRegisters()
        {
            AF = 0xFFFF;
            F = 0xFF;
            BC = 0xFFFF;
            DE = 0xFFFF;
            HL = 0xFFFF;
            PC = 0xFFFF;
            SP = 0xFFFF;
            _AF_ = 0xFFFF;
            _BC_ = 0xFFFF;
            _DE_ = 0xFFFF;
            _HL_ = 0xFFFF;
            IX = 0xFFFF;
            IY = 0xFFFF;
            IR = 0xFFFF;
            MW = 0xFFFF;
            IM = 0;
            IFF1 = IFF2 = 0;
            Halted = 0;
            Tacts = 0;
            FrameCount = 0;
            LastStepTacts = 0;
        }
    }
}