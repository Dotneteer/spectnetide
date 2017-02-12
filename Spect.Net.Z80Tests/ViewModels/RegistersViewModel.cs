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
            set { Set(ref _b, value); }
        }

        public byte C
        {
            get { return _c; }
            set { Set(ref _c, value); }
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

        public void Bind(Registers regs)
        {
            AF = regs.AF;
            BC = regs.BC;
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
                });
            }
        }
    }
}