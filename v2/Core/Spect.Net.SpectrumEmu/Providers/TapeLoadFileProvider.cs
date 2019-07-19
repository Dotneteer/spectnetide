using System.IO;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Tape;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This is the default tape provider used by the scripting engine
    /// </summary>
    public class TapeLoadFileProvider : ITapeLoadProvider
    {
        /// <summary>
        /// The component provider should be able to reset itself
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// The virtual machine that hosts the provider
        /// </summary>
        public ISpectrumVm HostVm { get; set; }

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
        }

        /// <summary>
        /// Gets a binary reader that provider TZX content
        /// </summary>
        /// <returns>BinaryReader instance to obtain the content from</returns>
        public BinaryReader GetTapeContent()
        {
            return null;
        }

        /// <summary>
        /// Creates a tape file with the specified name
        /// </summary>
        /// <returns></returns>
        public void CreateTapeFile()
        {
        }

        /// <summary>
        /// This method sets the name of the file according to the 
        /// Spectrum SAVE HEADER information
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
        }

        /// <summary>
        /// Appends the TZX block to the tape file
        /// </summary>
        /// <param name="block"></param>
        public void SaveTapeBlock(ITapeDataSerialization block)
        {
        }
    }

}