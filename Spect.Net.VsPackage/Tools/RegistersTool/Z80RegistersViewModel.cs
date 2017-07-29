using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.Wpf.Mvvm;
// ReSharper disable InconsistentNaming

namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// This view model represents the set of Z80 registers
    /// </summary>
    public class Z80RegistersViewModel: EnhancedViewModelBase
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