using System;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.ToolWindows.TestExplorer;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Debugging;

namespace Spect.Net.VsPackage
{
    public interface ISpectNetPackage
    {
        /// <summary>
        /// Represents the application object through which VS automation
        /// can be accessed.
        /// </summary>
        DTE2 ApplicationObject { get; }

        /// <summary>
        /// Service provider object
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Keeps the currently loaded solution structure
        /// </summary>
        SolutionStructure CodeDiscoverySolution { get; }

        /// <summary>
        /// The view model of the spectrum emulator
        /// </summary>
        MachineViewModel MachineViewModel { get; }

        /// <summary>
        /// The object responsible for managing Z80 program files
        /// </summary>
        Z80CodeManager CodeManager { get; }

        /// <summary>
        /// The object responsible for managing Z80 unit test files
        /// </summary>
        Z80TestManager TestManager { get; }

        /// <summary>
        /// The object responsible for managing VMState files
        /// </summary>
        VmStateFileManager StateFileManager { get; }

        /// <summary>
        /// Provides debug information while running the Spectrum virtual machine
        /// </summary>
        VsIntegratedSpectrumDebugInfoProvider DebugInfoProvider { get; }

        /// <summary>
        /// The error list provider accessible from this package
        /// </summary>
        ErrorListWindow ErrorList { get; }

        /// <summary>
        /// Gets the task list provider for this package.
        /// </summary>
        TaskListWindow TaskList { get; }

        /// <summary>
        /// Gets the options of this package
        /// </summary>
        SpectNetOptionsGrid Options { get; }

        /// <summary>
        /// This event fires when the package has opened a solution and
        /// prepared the virtual machine to work with
        /// </summary>
        event EventHandler SolutionOpened;

        /// <summary>
        /// This event fires when the solution has been closed
        /// </summary>
        event EventHandler SolutionClosed;

        /// <summary>
        /// Signs that the package started closing
        /// </summary>
        event EventHandler PackageClosing;

        /// <summary>
        /// Signs that a test file has been changed;
        /// </summary>
        event EventHandler<FileChangedEventArgs> TestFileChanged;

        /// <summary>
        /// Displays the tool window with the specified type
        /// </summary>
        /// <typeparam name="TWindow">Tool window type</typeparam>
        void ShowToolWindow<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane;

        /// <summary>
        /// Initializes the non-VS-integrated members of the class
        /// </summary>
        void InitMembers();

        /// <summary>
        /// Signs the the specified test file has been changed
        /// </summary>
        /// <param name="filename">Test file name</param>
        void OnTestFileChanged(string filename);

        /// <summary>
        /// Gets the specified tool window with the given instance id.
        /// </summary>
        /// <typeparam name="TWindow">Type of the tool window</typeparam>
        /// <param name="instanceId">Tool window insatnce ID</param>
        /// <returns>Tool window instance</returns>
        TWindow GetToolWindow<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane;
    }
}