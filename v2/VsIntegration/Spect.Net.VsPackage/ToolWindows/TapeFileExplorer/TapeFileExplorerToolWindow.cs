using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.ToolWindows.TapeFileExplorer
{
    /// <summary>
    /// This class implements the TZX Explorer tool window.
    /// </summary>
    [Guid("CA2EC5DC-AA73-4BF8-97F0-3F2CC81E2EE0")]
    [Caption("Tape File Explorer")]
    [ToolWindowToolbar(0x1410)]
    public class TapeFileExplorerToolWindow : ToolWindowPaneBase<TapeFileExplorerControl>
    {
        /// <summary>
        /// TZX file filter string
        /// </summary>
        public const string TZX_FILTER = "TZX Files (*.tzx)|*.tzx|TAP Files (*.tap)|*.tap";

        /// <summary>
        /// Invoke this method from the main thread to initialize toolbar commands.
        /// </summary>
        public static void InitializeToolbarCommands()
        {
            new OpenTzxFileCommand();
        }

        /// <summary>
        /// Displays the open TZX file dialog
        /// </summary>
        [CommandId(0x1481)]
        public class OpenTzxFileCommand : SpectNetCommandBase
        {
            protected override Task ExecuteAsync()
            {
                var tw = SpectNetPackage.Default.GetToolWindow<TapeFileExplorerToolWindow>();
                var vm = tw.Content.Vm;
                var filename = VsxDialogs.FileOpen(TZX_FILTER, vm.LatestPath);
                if (filename == null) return Task.FromResult(0);

                vm.FileName = filename;
                vm.LatestPath = Path.GetDirectoryName(filename);
                vm.ReadFrom(filename);
                vm.Loaded = true;
                tw.Content.EditorControl.Vm = vm;
                if (vm.Blocks.Count > 0)
                {
                    vm.Blocks[0].IsSelected = true;
                }
                vm.BlockSelectedCommand.Execute(null);
                return Task.FromResult(0);
            }
        }
    }
}
