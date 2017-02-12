using GalaSoft.MvvmLight;
using Spect.Net.Z80Emu.Core;

// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Tests.ViewModels
{
    public class RegistersViewModel: ViewModelBase
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


        public byte A
        {
            get { return _a; }
            set
            {
                Set(ref _a, value);
                RaisePropertyChanged("AF");
            }
        }

        public byte F
        {
            get { return _f; }
            set
            {
                Set(ref _f, value);
                RaisePropertyChanged("AF");
            }
        }

        public ushort AF
        {
            get { return (ushort) (_a << 8 | _f); }
            set
            {
                A = (byte) (value >> 8);
                F = (byte) (value & 0xFF);
            }
        }

        public byte B
        {
            get { return _b; }
            set
            {
                Set(ref _b, value);
                RaisePropertyChanged("BC");
            }
        }

        public byte C
        {
            get { return _c; }
            set
            {
                Set(ref _c, value);
                RaisePropertyChanged("BC");

            }
        }

        public ushort BC
        {
            get { return (ushort)(_b << 8 | _c); }
            set
            {
                B = (byte)(value >> 8);
                C = (byte)(value & 0xFF);
            }
        }

        public byte D
        {
            get { return _d; }
            set
            {
                Set(ref _d, value);
                RaisePropertyChanged("DE");
            }
        }

        public byte E
        {
            get { return _e; }
            set
            {
                Set(ref _e, value);
                RaisePropertyChanged("DE");

            }
        }

        public ushort DE
        {
            get { return (ushort)(_d << 8 | _e); }
            set
            {
                D = (byte)(value >> 8);
                E = (byte)(value & 0xFF);
            }
        }

        public byte H
        {
            get { return _h; }
            set
            {
                Set(ref _h, value);
                RaisePropertyChanged("DE");
            }
        }

        public byte L
        {
            get { return _l; }
            set
            {
                Set(ref _l, value);
                RaisePropertyChanged("DE");

            }
        }

        public ushort HL
        {
            get { return (ushort)(_h << 8 | _l); }
            set
            {
                H = (byte)(value >> 8);
                L = (byte)(value & 0xFF);
            }
        }

        public ushort PC
        {
            get { return _pc; }
            set { Set(ref _pc, value); }
        }

        public ushort SP
        {
            get { return _sp; }
            set { Set(ref _sp, value); }
        }

        public ushort IX
        {
            get { return _ix; }
            set { Set(ref _ix, value); }
        }

        public ushort IY
        {
            get { return _iy; }
            set { Set(ref _iy, value); }
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
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public RegistersViewModel()
        {
            if (IsInDesignMode)
            {
                Bind(new Registers
                {
                    AF = 0xAAAA,
                    BC = 0x5555,
                    DE = 0xEEEE
                });
            }
        }
    }
}