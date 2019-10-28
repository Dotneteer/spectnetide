using System.IO;
using System.Reflection;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Providers
{
    /// <summary>
    /// This is the default tape provider used by the scripting engine
    /// </summary>
    public class ResourceBasedTapeLoadProvider : ITapeLoadProvider
    {
        /// <summary>
        /// This class implements a tape load provider that uses embedded resources
        /// </summary>
        /// <param name="asm">Host assembly of resources</param>
        public ResourceBasedTapeLoadProvider(Assembly asm = null)
        {
            Assembly = asm ?? Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// The assembly that contains the resource
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// The name of the resource that contains the tape file
        /// </summary>
        public string ResourceName { get; set; }

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
            var resourceName = $"{Assembly.GetName().Name}.{ResourceName}";
            var stream = Assembly.GetManifestResourceStream(resourceName);
            // ReSharper disable once AssignNullToNotNullAttribute
            return new BinaryReader(stream);
        }
    }

}