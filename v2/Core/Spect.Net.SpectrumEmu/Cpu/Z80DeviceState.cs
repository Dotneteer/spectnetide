// ReSharper disable InconsistentNaming

using Spect.Net.SpectrumEmu.Abstraction.Cpu;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Cpu
{
    public partial class Z80Cpu
    {
        /// <summary>
        /// This class descibes the state of the Z80 CPU
        /// </summary>
        public class Z80DeviceState : IDeviceState
        {
            public bool AllowExtendedInstructionSet { get; set; }
            public long Tacts { get; set; }
            public Registers Registers { get; set; }
            public Z80StateFlags StateFlags { get; set; }
            public bool UseGateArrayContention { get; set; }
            public bool IFF1 { get; set; }
            public bool IFF2 { get; set; }
            public byte InterruptMode { get; set; }
            public bool MaskableInterruptModeEntered { get; set; }

            /// <summary>
            /// Allows a deserializer to create an empty state object
            /// </summary>
            public Z80DeviceState()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object" /> class.
            /// </summary>
            public Z80DeviceState(Z80Cpu cpu)
            {
                if (cpu == null) return;
                AllowExtendedInstructionSet = cpu.AllowExtendedInstructionSet;
                Tacts = cpu.Tacts;
                Registers = cpu.Registers;
                StateFlags = cpu.StateFlags;
                UseGateArrayContention = cpu.UseGateArrayContention;
                IFF1 = cpu.IFF1;
                IFF2 = cpu.IFF2;
                InterruptMode = cpu.InterruptMode;
                MaskableInterruptModeEntered = cpu.MaskableInterruptModeEntered;

            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is Z80Cpu cpu)) return;

                cpu.AllowExtendedInstructionSet = AllowExtendedInstructionSet;
                cpu._tacts = Tacts;
                cpu._registers = Registers;
                cpu.StateFlags = StateFlags;
                cpu.UseGateArrayContention = UseGateArrayContention;
                cpu.IFF1 = IFF1;
                cpu.IFF2 = IFF2;
                cpu._interruptMode = InterruptMode;
                cpu.MaskableInterruptModeEntered = MaskableInterruptModeEntered;
            }
        }
    }
}