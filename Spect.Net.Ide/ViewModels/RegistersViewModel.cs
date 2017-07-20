// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable InconsistentNaming

using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.Ide.ViewModels
{
    public class RegistersViewModel: ToolWindowViewModel
    {
        private byte _a;
        private byte _f;
        private byte _b;
        private byte _c;
        private byte _d;
        private byte _e;
        private byte _h;
        private byte _l;
        private ushort _sp;
        private ushort _pc;
        private ushort _ix;
        private ushort _iy;
        private ushort _ir;
        private byte _a_;
        private byte _f_;
        private byte _b_;
        private byte _c_;
        private byte _d_;
        private byte _e_;
        private byte _h_;
        private byte _l_;

        public byte A
        {
            get => _a;
            set
            {
                Set(ref _a, value);
                RaisePropertyChanged("AF");
            }
        }

        public byte F
        {
            get => _f;
            set
            {
                Set(ref _f, value);
                RaisePropertyChanged("AF");
            }
        }

        public ushort AF
        {
            get => (ushort) (_a << 8 | _f);
            set
            {
                A = (byte) (value >> 8);
                F = (byte) (value & 0xFF);
            }
        }

        public byte B
        {
            get => _b;
            set
            {
                Set(ref _b, value);
                RaisePropertyChanged("BC");
            }
        }

        public byte C
        {
            get => _c;
            set
            {
                Set(ref _c, value);
                RaisePropertyChanged("BC");

            }
        }

        public ushort BC
        {
            get => (ushort)(_b << 8 | _c);
            set
            {
                B = (byte)(value >> 8);
                C = (byte)(value & 0xFF);
            }
        }

        public byte D
        {
            get => _d;
            set
            {
                Set(ref _d, value);
                RaisePropertyChanged("DE");
            }
        }

        public byte E
        {
            get => _e;
            set
            {
                Set(ref _e, value);
                RaisePropertyChanged("DE");

            }
        }

        public ushort DE
        {
            get => (ushort)(_d << 8 | _e);
            set
            {
                D = (byte)(value >> 8);
                E = (byte)(value & 0xFF);
            }
        }

        public byte H
        {
            get => _h;
            set
            {
                Set(ref _h, value);
                RaisePropertyChanged("HL");
            }
        }

        public byte L
        {
            get => _l;
            set
            {
                Set(ref _l, value);
                RaisePropertyChanged("HL");

            }
        }

        public ushort HL
        {
            get => (ushort)(_h << 8 | _l);
            set
            {
                H = (byte)(value >> 8);
                L = (byte)(value & 0xFF);
            }
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

        public byte A_
        {
            get => _a_;
            set
            {
                Set(ref _a_, value);
                RaisePropertyChanged("AF_");
            }
        }

        public byte F_
        {
            get => _f_;
            set
            {
                Set(ref _f_, value);
                RaisePropertyChanged("AF_");
            }
        }

        public ushort AF_
        {
            get => (ushort)(_a_ << 8 | _f_);
            set
            {
                A_ = (byte)(value >> 8);
                F_ = (byte)(value & 0xFF);
            }
        }

        public byte B_
        {
            get => _b_;
            set
            {
                Set(ref _b_, value);
                RaisePropertyChanged("BC_");
            }
        }

        public byte C_
        {
            get => _c_;
            set
            {
                Set(ref _c_, value);
                RaisePropertyChanged("BC_");

            }
        }

        public ushort BC_
        {
            get => (ushort)(_b_ << 8 | _c_);
            set
            {
                B_ = (byte)(value >> 8);
                C_ = (byte)(value & 0xFF);
            }
        }

        public byte D_
        {
            get => _d_;
            set
            {
                Set(ref _d_, value);
                RaisePropertyChanged("DE_");
            }
        }

        public byte E_
        {
            get => _e_;
            set
            {
                Set(ref _e_, value);
                RaisePropertyChanged("DE_");

            }
        }

        public ushort DE_
        {
            get => (ushort)(_d_ << 8 | _e_);
            set
            {
                D_ = (byte)(value >> 8);
                E_ = (byte)(value & 0xFF);
            }
        }

        public byte H_
        {
            get => _h_;
            set
            {
                Set(ref _h_, value);
                RaisePropertyChanged("HL_");
            }
        }

        public byte L_
        {
            get => _l_;
            set
            {
                Set(ref _l_, value);
                RaisePropertyChanged("DE");

            }
        }

        public ushort HL_
        {
            get => (ushort)(_h_ << 8 | _l_);
            set
            {
                H_ = (byte)(value >> 8);
                L_ = (byte)(value & 0xFF);
            }
        }

        public void Bind(Registers regs)
        {
            AF = regs.AF;
            BC = regs.BC;
            DE = regs.DE;
            HL = regs.HL;
            PC = regs.PC;
            SP = regs.SP;
            IX = regs.IX;
            IY = regs.IY;
            IR = regs.IR;
            AF_ = regs._AF_;
            BC_ = regs._BC_;
            DE_ = regs._DE_;
            HL_ = regs._HL_;
        }

        public RegistersViewModel()
        {
            Title = "Registers";
        }
    }
}