using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Devices.Sound;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface represents the PSG device of a Spectrum 128/+2/+3
    /// virtual machine
    /// </summary>
    public interface ISoundDevice: IFrameBoundDevice, ISpectrumBoundDevice
    {
        /// <summary>
        /// The PSG state snapshots collected during the last frame
        /// </summary>
        List<PsgStateSnapshot> PsgSnapshots { get; }

        /// <summary>
        /// The last PSG state collected during the last frame
        /// </summary>
        PsgState LastPsgState { get; }

        /// <summary>
        /// The index of the last addressed register
        /// </summary>
        byte LastRegisterIndex { get; }

        /// <summary>
        /// Sets the index of the PSG register
        /// </summary>
        /// <param name="index">Register index</param>
        void SetRegisterIndex(byte index);

        /// <summary>
        /// Sets the value of the register according to the
        /// last register index
        /// </summary>
        /// <param name="value">Register value</param>
        void SetRegisterValue(byte value);

        /// <summary>
        /// Gets the value of the register according to the
        /// last register index
        /// </summary>
        /// <returns>Register value</returns>
        byte GetRegisterValue();
    }
}