// ReSharper disable InconsistentNaming

using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Cpu;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class represents the Z80 CPU of a Spectrum virtual machine
    /// </summary>
    public sealed class CpuZ80
    {
        private readonly IZ80Cpu _cpu;

        /// <summary>
        /// Binds this instance to the specified Z80 CPU instance
        /// </summary>
        /// <param name="cpu"></param>
        internal CpuZ80(IZ80Cpu cpu)
        {
            _cpu = cpu;
            if (!(cpu is IZ80CpuTestSupport runSupport))
            {
                throw new ArgumentException("The cpu instance should implement IZ80CpuTestSupport", 
                    nameof(cpu));
            }
            OperationTrackingState = new AddressTrackingState(runSupport.ExecutionFlowStatus);
        }

        /// <summary>
        /// AF register pair
        /// </summary>
        public ushort AF
        {
            get => _cpu.Registers.AF;
            set => _cpu.Registers.AF = value;
        }

        /// <summary>
        /// BC register pair
        /// </summary>
        public ushort BC
        {
            get => _cpu.Registers.BC;
            set => _cpu.Registers.BC = value;
        }

        /// <summary>
        /// DE register pair
        /// </summary>
        public ushort DE
        {
            get => _cpu.Registers.DE;
            set => _cpu.Registers.DE = value;
        }

        /// <summary>
        /// HL register pair
        /// </summary>
        public ushort HL
        {
            get => _cpu.Registers.HL;
            set => _cpu.Registers.HL = value;
        }

        /// <summary>
        /// AF' register pair
        /// </summary>
        public ushort _AF_
        {
            get => _cpu.Registers._AF_;
            set => _cpu.Registers._AF_ = value;
        }

        /// <summary>
        /// BC' register pair
        /// </summary>
        public ushort _BC_
        {
            get => _cpu.Registers._BC_;
            set => _cpu.Registers._BC_ = value;
        }

        /// <summary>
        /// DE' register pair
        /// </summary>
        public ushort _DE_
        {
            get => _cpu.Registers._DE_;
            set => _cpu.Registers._DE_ = value;
        }

        /// <summary>
        /// HL' register pair
        /// </summary>
        public ushort _HL_
        {
            get => _cpu.Registers._HL_;
            set => _cpu.Registers._HL_ = value;
        }

        /// <summary>
        /// IX register pair
        /// </summary>
        public ushort IX
        {
            get => _cpu.Registers.IX;
            set => _cpu.Registers.IX = value;
        }

        /// <summary>
        /// IY register pair
        /// </summary>
        public ushort IY
        {
            get => _cpu.Registers.IY;
            set => _cpu.Registers.IY = value;
        }

        /// <summary>
        /// Interrupt Page Address (I) Register/Memory Refresh (R) Register
        /// </summary>
        public ushort IR
        {
            get => _cpu.Registers.IR;
            set => _cpu.Registers.IR = value;
        }

        /// <summary>
        /// Program Counter
        /// </summary>
        public ushort PC
        {
            get => _cpu.Registers.PC;
            set => _cpu.Registers.PC = value;
        }

        /// <summary>
        /// Stack Pointer
        /// </summary>
        public ushort SP
        {
            get => _cpu.Registers.SP;
            set => _cpu.Registers.SP = value;
        }

        /// <summary>
        /// Internal register WZ to support 16-bit addressing operations
        /// </summary>
        public ushort WZ => _cpu.Registers.WZ;

        /// <summary>
        /// Accumulator
        /// </summary>
        public byte A
        {
            get => _cpu.Registers.A;
            set => _cpu.Registers.A = value;
        }

        /// <summary>
        /// Flags
        /// </summary>
        public byte F
        {
            get => _cpu.Registers.F;
            set => _cpu.Registers.F = value;
        }

        /// <summary>
        /// General purpose register B
        /// </summary>
        public byte B
        {
            get => _cpu.Registers.B;
            set => _cpu.Registers.B = value;
        }

        /// <summary>
        /// General purpose register C
        /// </summary>
        public byte C
        {
            get => _cpu.Registers.C;
            set => _cpu.Registers.C = value;
        }

        /// <summary>
        /// General purpose register D
        /// </summary>
        public byte D
        {
            get => _cpu.Registers.D;
            set => _cpu.Registers.D = value;
        }

        /// <summary>
        /// General purpose register E
        /// </summary>
        public byte E
        {
            get => _cpu.Registers.E;
            set => _cpu.Registers.E = value;
        }

        /// <summary>
        /// General purpose register H
        /// </summary>
        public byte H
        {
            get => _cpu.Registers.H;
            set => _cpu.Registers.H = value;
        }

        /// <summary>
        /// General purpose register L
        /// </summary>
        public byte L
        {
            get => _cpu.Registers.L;
            set => _cpu.Registers.L = value;
        }

        /// <summary>
        /// High 8-bit of IX
        /// </summary>
        public byte XH
        {
            get => _cpu.Registers.XH;
            set => _cpu.Registers.XH = value;
        }

        /// <summary>
        /// Low 8-bit of IX
        /// </summary>
        public byte XL
        {
            get => _cpu.Registers.XL;
            set => _cpu.Registers.XL = value;
        }

        /// <summary>
        /// High 8-bit of IY
        /// </summary>
        public byte YH
        {
            get => _cpu.Registers.YH;
            set => _cpu.Registers.YH = value;
        }

        /// <summary>
        /// High 8-bit of IY
        /// </summary>
        public byte YL
        {
            get => _cpu.Registers.YL;
            set => _cpu.Registers.YL = value;
        }

        /// <summary>
        /// Interrupt Page Address (I) Register
        /// </summary>
        public byte I
        {
            get => _cpu.Registers.I;
            set => _cpu.Registers.I = value;
        }

        /// <summary>
        /// Memory Refresh (R) Register
        /// </summary>
        public byte R
        {
            get => _cpu.Registers.R;
            set => _cpu.Registers.R = value;
        }

        /// <summary>
        /// High 8-bit of WZ
        /// </summary>
        public byte WZh => _cpu.Registers.WZh;


        /// <summary>
        /// Low 8-bit of WZ
        /// </summary>
        public byte WZl => _cpu.Registers.WZl;

        /// <summary>
        /// S Flag
        /// </summary>
        public bool SFlag
        {
            get => _cpu.Registers.SFlag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.S) | (value ? FlagsSetMask.S : 0));
        }

        /// <summary>
        /// Z Flag
        /// </summary>
        public bool ZFlag
        {
            get => _cpu.Registers.ZFlag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.Z) | (value ? FlagsSetMask.Z : 0));
        }


        /// <summary>
        /// R5 Flag
        /// </summary>
        public bool R5Flag
        {
            get => _cpu.Registers.R5Flag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.R5) | (value ? FlagsSetMask.R5 : 0));
        }

        /// <summary>
        /// H Flag
        /// </summary>
        public bool HFlag
        {
            get => _cpu.Registers.HFlag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.H) | (value ? FlagsSetMask.H : 0));
        }

        /// <summary>
        /// R3 Flag
        /// </summary>
        public bool R3Flag
        {
            get => _cpu.Registers.R3Flag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.R3) | (value ? FlagsSetMask.R3 : 0));
        }

        /// <summary>
        /// P/V Flag
        /// </summary>
        public bool PVFlag
        {
            get => _cpu.Registers.PFlag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.PV) | (value ? FlagsSetMask.PV : 0));
        }

        /// <summary>
        /// N Flag
        /// </summary>
        public bool NFlag
        {
            get => _cpu.Registers.NFlag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.N) | (value ? FlagsSetMask.N : 0));
        }

        /// <summary>
        /// C Flag
        /// </summary>
        public bool CFlag
        {
            get => _cpu.Registers.CFlag;
            set => _cpu.Registers.F = (byte)((_cpu.Registers.F & FlagsResetMask.C) | (value ? FlagsSetMask.C : 0));
        }

        /// <summary>
        /// Gets the current tacts of the CPU -- the clock cycles since
        /// the CPU was reset
        /// </summary>
        public long Tacts => _cpu.Tacts;

        /// <summary>
        /// Interrupt Enable Flip-Flop #1
        /// </summary>
        /// <remarks>
        /// Disables interrupts from being accepted 
        /// </remarks>
        public bool IFF1 => _cpu.IFF1;

        /// <summary>
        /// Interrupt Enable Flip-Flop #2
        /// </summary>
        /// <remarks>
        /// Temporary storage location for IFF1
        /// </remarks>
        public bool IFF2 => _cpu.IFF2;

        /// <summary>
        /// The current Interrupt mode
        /// </summary>
        /// <remarks>
        /// IM 0 / IM 1 / IM 2
        /// </remarks>
        public byte InterruptMode => _cpu.InterruptMode;

        /// <summary>
        /// The interrupt is blocked
        /// </summary>
        public bool IsInterruptBlocked => _cpu.IsInterruptBlocked;

        /// <summary>
        /// Is currently in opcode execution?
        /// </summary>
        public bool IsInOpExecution => _cpu.IsInOpExecution;

        /// <summary>
        /// This flag indicates if the CPU entered into a maskable
        /// interrupt method as a result of an INT signal
        /// </summary>
        public bool MaskableInterruptModeEntered => _cpu.MaskableInterruptModeEntered;

        /// <summary>
        /// Resets the CPU
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// Disables the maskable interrupt
        /// </summary>
        public void DisableInterrupt()
        {
        }

        /// <summary>
        /// Enables the maskable interrupt
        /// </summary>
        public void EnableInterrupt()
        {
        }

        /// <summary>
        /// Resets the operation tracking information
        /// </summary>
        public void ResetOperationTracking()
        {
            OperationTrackingState.Clear();
        }

        /// <summary>
        /// Gets the operation tracking state information
        /// </summary>
        public AddressTrackingState OperationTrackingState { get; }
    }
}