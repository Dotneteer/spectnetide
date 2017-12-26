using System;
using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Sound
{
    public class SoundDevice: ISoundDevice
    {
        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// The PSG state snapshots collected during the last frame
        /// </summary>
        public List<PsgStateSnapshot> PsgSnapshots { get; private set;  }

        /// <summary>
        /// The last PSG state collected during the last frame
        /// </summary>
        public PsgState LastPsgState { get; private set; }

        /// <summary>
        /// The index of the last addressed register
        /// </summary>
        public byte LastRegisterIndex { get; private set; }

        /// <summary>
        /// Sets the index of the PSG register
        /// </summary>
        /// <param name="index">Register index</param>
        public void SetRegisterIndex(byte index)
        {
            LastRegisterIndex = index;
        }

        /// <summary>
        /// Sets the value of the register according to the
        /// last register index
        /// </summary>
        /// <param name="value">Register value</param>
        public void SetRegisterValue(byte value)
        {
        }
    }
}