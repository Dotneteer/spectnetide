using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class implements the state file manager for the scriptable SpectrumVm class
    /// </summary>
    public class SpectrumVmStateFileManager : SpectrumVmStateFileManagerBase
    {
        private readonly Func<string> _cacheFolderFunc;

        public SpectrumVmStateFileManager(string modelName, ISpectrumVm spectrum, ISpectrumVmController controller,
            Func<string> cacheFolderFunc)
        {
            ModelName = modelName;
            SpectrumVm = spectrum;
            VmController = controller;
            _cacheFolderFunc = cacheFolderFunc;
        }

        /// <summary>
        /// Obtains the current model's name
        /// </summary>
        protected override string ModelName { get; }

        /// <summary>
        /// Obtains the Spectrum virtual machine controller this manager is bound to
        /// </summary>
        protected override ISpectrumVmController VmController { get; }

        /// <summary>
        /// Obtains the Spectrum virtual machine this manager is bound to
        /// </summary>
        protected override ISpectrumVm SpectrumVm { get; }

        /// <summary>
        /// Get the name of the folder to save/load machine state files
        /// </summary>
        /// <returns></returns>
        protected override string GetStateFolder() => _cacheFolderFunc();

        /// <summary>
        /// Forces the virtual machine to paused state
        /// </summary>
        protected override void ForcePausedState()
        {
            VmController.ForcePausedState();
        }

        /// <summary>
        /// Define how to reset devices after load
        /// </summary>
        protected override void ResetDevicesAfterLoad()
        {
            SpectrumVm.BeeperDevice.Reset();
            SpectrumVm.BeeperProvider.Reset();
        }
    }
}