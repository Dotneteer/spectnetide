using System.Collections.Generic;
using System.Threading.Tasks;
using EnvDTE;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.VsPackage.Machines
{
    /// <summary>
    /// This class represents the machines associated with the project within the solution
    /// </summary>
    public class MachineCollection
    {
        // --- The machines for this solution
        private readonly Dictionary<Project, SpectrumMachine> _machines = 
            new Dictionary<Project, SpectrumMachine>();

        /// <summary>
        /// Gets the virtual machine for the specified project
        /// </summary>
        /// <param name="project">Project to get the machine for</param>
        /// <returns>ZX Spectrum virtual machine of the project</returns>
        public SpectrumMachine GetMachine(Project project)
        {
            return _machines.TryGetValue(project, out var machine) 
                ? machine : null;
        }

        /// <summary>
        /// Creates a machine for the specified project
        /// </summary>
        /// <param name="project">Project to create a machine for</param>
        /// <param name="modelKey">ZX Spectrum model key</param>
        /// <param name="editionKey">ZX Spectrum edition key</param>
        /// <returns>ZX Spectrum virtual machine of the project</returns>
        public SpectrumMachine GetOrCreateMachine(Project project, string modelKey, string editionKey)
        {
            var machine = GetMachine(project);
            if (machine != null) return machine;

            machine = SpectrumMachine.CreateMachine(modelKey, editionKey) as SpectrumMachine;
            _machines[project] = machine;
            return machine;
        }

        /// <summary>
        /// Removes the virtual machine for the specified project
        /// </summary>
        /// <param name="project"></param>
        public async Task DisposeMachineAsync(Project project)
        {
            var machine = GetMachine(project);
            if (machine == null) return;

            await machine.Stop();
            _machines.Remove(project);
        }

        /// <summary>
        /// Erases all virtual machines
        /// </summary>
        public async Task DisposeMachinesAsync()
        {
            foreach (var machine in _machines.Values)
            {
                await machine.Stop();
            }
            _machines.Clear();
        }
    }
}