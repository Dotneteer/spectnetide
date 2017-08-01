using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.Wpf.SpectrumControl;

// ReSharper disable InconsistentNaming

namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// This view model represents the set of Z80 registers
    /// </summary>
    public class Z80RegistersViewModel: SpectrumGenericToolWindowViewModel
    {
        private ushort _af;
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

        private long _tacts;

        public ushort AF
        {
            get => _af;
            set => Set(ref _af, value);
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

        public long Tacts
        {
            get => _tacts;
            set => Set(ref _tacts, value);
        }

        /// <summary>
        /// Instantiates this view model
        /// </summary>
        public Z80RegistersViewModel()
        {
            AF = 0xFFFF;
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
            Tacts = 0;
        }

        /// <summary>
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(SpectrumVmStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);
            if (VmPaused)
            {
                BindTo(SpectrumVmViewModel.SpectrumVm.Cpu);
            }
        }

        /// <summary>
        /// Set the machine status when the screen has been refreshed
        /// </summary>
        protected override void OnScreenRefreshed(SpectrumScreenRefreshedMessage msg)
        {
            base.OnScreenRefreshed(msg);
            if (ScreenRefreshCount % 4 == 0)
            {
                BindTo(SpectrumVmViewModel.SpectrumVm.Cpu);
            }
        }

        /// <summary>
        /// Bind these registers to the Z80 CPU's register values
        /// </summary>
        public void BindTo(IZ80Cpu cpu)
        {
            var regs = cpu.Registers;
            AF = regs.AF;
            BC = regs.BC;
            DE = regs.DE;
            HL = regs.HL;
            _AF_ = regs._AF_;
            _BC_ = regs._BC_;
            _DE_ = regs._DE_;
            _HL_ = regs._HL_;
            PC = regs.PC;
            SP = regs.SP;
            IX = regs.IX;
            IY = regs.IY;
            IR = regs.IR;
            MW = regs.MW;

            IM = cpu.InterruptMode;
            IFF1 = cpu.IFF1 ? 1 : 0;
            IFF2 = cpu.IFF2 ? 1 : 0;
            Tacts = cpu.Tacts;
        }
    }
}