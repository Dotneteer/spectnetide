using System.IO;
using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.TapeFileExplorer
{
    /// <summary>
    /// This class implements the TZX Explorer tool window.
    /// </summary>
    [Guid("CA2EC5DC-AA73-4BF8-97F0-3F2CC81E2EE0")]
    [Caption("Tape File Explorer")]
    [ToolWindowToolbar(typeof(SpectNetCommandSet), 0x1410)]
    public class TapeFileExplorerToolWindow : VsxToolWindowPane<TapeFileExplorerControl>
    {
        /// <summary>
        /// TZX file filter string
        /// </summary>
        public const string TZX_FILTER = "TZX Files (*.tzx)|*.tzx|TAP Files (*.tap)|*.tap";

        /// <summary>
        /// Displays the open TZX file dialog
        /// </summary>
        [CommandId(0x1481)]
        public class OpenTzxFileCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                var tw = Package.GetToolWindow<TapeFileExplorerToolWindow>();
                var vm = tw.Content.Vm;
                var filename = VsxDialogs.FileOpen(TZX_FILTER, vm.LatestPath);
                if (filename == null) return;

                vm.FileName = filename;
                vm.LatestPath = Path.GetDirectoryName(filename);
                vm.ReadFrom(filename);
                vm.Loaded = true;
                tw.Content.EditorControl.Vm = vm;
            }
        }
    }
}
