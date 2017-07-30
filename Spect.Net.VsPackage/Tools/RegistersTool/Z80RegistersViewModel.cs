using System.Windows.Threading;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.Wpf.SpectrumControl;

// ReSharper disable InconsistentNaming

namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// This view model represents the set of Z80 registers
    /// </summary>
    public class Z80RegistersViewModel: SpectrumToolWindowViewModelBase
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
        }

        /// <summary>
        /// Set the machnine status
        /// </summary>
        protected override void OnVmStateChanged(SpectrumVmStateChangedMessage msg)
        {
            base.OnVmStateChanged(msg);
            if (VmPaused)
            {
                BindTo(SpectrumVmViewModel.SpectrumVm.Cpu.Registers);
            }
        }

        /// <summary>
        /// Bind these registers to the Z80 CPU's register values
        /// </summary>
        /// <param name="z80Regs"></param>
        public void BindTo(Registers z80Regs)
        {
            AF = z80Regs.AF;
            BC = z80Regs.BC;
            DE = z80Regs.DE;
            HL = z80Regs.HL;
            _AF_ = z80Regs._AF_;
            _BC_ = z80Regs._BC_;
            _DE_ = z80Regs._DE_;
            _HL_ = z80Regs._HL_;
            PC = z80Regs.PC;
            SP = z80Regs.SP;
            IX = z80Regs.IX;
            IY = z80Regs.IY;
            IR = z80Regs.IR;
            MW = z80Regs.MW;
        }
    }
}