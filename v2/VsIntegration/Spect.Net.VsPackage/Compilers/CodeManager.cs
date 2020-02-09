using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Machine;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary.Output;
using OutputWindow = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the events related to code management
    /// </summary>
    public class CodeManager
    {
        /// <summary>
        /// This event signs that code has been injected into the virtual machine.
        /// </summary>
        public event EventHandler CodeInjected;

        /// <summary>
        /// Signs that the compilation has completed
        /// </summary>
        public event EventHandler<CompilationCompletedEventArgs> CompilationCompleted;

        /// <summary>
        /// Signs that the annotation file has been changed
        /// </summary>
        public event EventHandler AnnotationFileChanged;

        /// <summary>
        /// Signs that the code compilation completed.
        /// </summary>
        /// <param name="output">Assembler output</param>
        public void RaiseCompilationCompleted(AssemblerOutput output)
        {
            CompilationCompleted?.Invoke(this, new CompilationCompletedEventArgs(output));
        }

        /// <summary>
        /// Signs that the annotation file has been changed.
        /// </summary>
        public void RaiseAnnotationFileChanged()
        {
            AnnotationFileChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Injects the code into the Spectrum virtual machine's memory
        /// </summary>
        /// <param name="output"></param>
        public void InjectCodeIntoVm(AssemblerOutput output)
        {
            // --- Do not inject faulty code
            if (output == null || output.ErrorCount > 0)
            {
                return;
            }

            // --- Do not inject code if memory is not available
            var vm = SpectNetPackage.Default.EmulatorViewModel;
            var spectrumVm = vm.Machine.SpectrumVm;
            if (vm.MachineState != VmState.Paused || spectrumVm?.MemoryDevice == null)
            {
                return;
            }

            if (spectrumVm is ISpectrumVmRunCodeSupport runSupport)
            {
                // --- Go through all code segments and inject them
                foreach (var segment in output.Segments)
                {
                    var addr = segment.StartAddress + (segment.Displacement ?? 0);
                    runSupport.InjectCodeToMemory((ushort)addr, segment.EmittedCode);
                }

                // --- Prepare the machine for RUN mode
                runSupport.PrepareRunMode();
                CodeInjected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Runs the pre-build event
        /// </summary>
        /// <returns>True, if the event completed successfully.</returns>
        public async Task<bool> RunPreBuildEvent()
        {
            var config = GetSpectrumProjectConfiguration();
            if (config == null || string.IsNullOrWhiteSpace(config.PreBuildCommand))
            {
                return true;
            }
            return await RunCommand(config.PreBuildCommand);
        }

        /// <summary>
        /// Runs the post-build event
        /// </summary>
        /// <returns>True, if the event completed successfully.</returns>
        public async Task<bool> RunPostBuildEvent()
        {
            var config = GetSpectrumProjectConfiguration();
            if (config == null || string.IsNullOrWhiteSpace(config.PostBuildCommand))
            {
                return true;
            }
            return await RunCommand(config.PostBuildCommand);
        }

        /// <summary>
        /// Runs the build-error event
        /// </summary>
        /// <returns>True, if the event completed successfully.</returns>
        public async Task<bool> RunBuildErrorEvent()
        {
            var config = GetSpectrumProjectConfiguration();
            if (config == null || string.IsNullOrWhiteSpace(config.BuildErrorCommand))
            {
                return true;
            }
            return await RunCommand(config.BuildErrorCommand);
        }

        /// <summary>
        /// Gets the active project's configuration
        /// </summary>
        /// <returns>
        /// Configuration information
        /// </returns>
        public SpectrumProjectConfiguration GetSpectrumProjectConfiguration()
        {
            try
            {
                var configFileName = Path.Combine(SpectNetPackage.Default.ActiveProject.ProjectDir,
                    SpectrumProject.PROJECT_CONFIG_FILE);
                if (!File.Exists(configFileName)) return null;
                var configContents = File.ReadAllText(configFileName);
                return JsonConvert.DeserializeObject<SpectrumProjectConfiguration>(configContents);
            }
            catch (Exception ex)
            {
                var pane = OutputWindow.GetPane<Z80AssemblerOutputPane>();
                pane.WriteLine($"Cannot obtain ZX Spectrum project configuration: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Runs the specified command in a separate project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private Task<bool> RunCommand(string command)
        {
            var tcs = new TaskCompletionSource<bool>();
            var cmdProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine("cmd.exe"),
                    Arguments = command,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = SpectNetPackage.Default.ActiveProject.ProjectDir
                }
            };

            _ = Task.Factory.StartNew(() =>
            {
                try
                {
                    cmdProcess.Start();

                    // --- Wait up to 5 minutes to run the process
                    cmdProcess.WaitForExit(300000);
                    if (!cmdProcess.HasExited)
                    {
                        cmdProcess.Kill();
                        tcs.SetException(new InvalidOperationException("Task did not complete within timeout."));
                    }
                    else
                    {
                        var exitCode = cmdProcess.ExitCode;
                        var output = cmdProcess.StandardError.ReadToEnd();
                        var pane = OutputWindow.GetPane<Z80AssemblerOutputPane>();
                        pane.WriteLine(output);

                        tcs.SetResult(exitCode == 0);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                finally
                {
                    cmdProcess.Dispose();
                }
            });
            return tcs.Task;
        }
    }
}
