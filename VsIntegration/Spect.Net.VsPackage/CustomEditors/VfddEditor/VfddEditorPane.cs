using System;
using System.IO;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.VfddEditor
{
    /// <summary>
    /// Editor pane class for virtual floppy disk file viewer
    /// </summary>
    public class VfddEditorPane : EditorPaneBase<VfddEditorFactory, VfddEditorControl>
    {
        private string _loadPath;

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public override string FileExtensionUsed => ".vfdd";

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName"></param>
        protected override void LoadFile(string fileName)
        {
            _loadPath = fileName;
            var vm = new VfddEditorViewModel();
            try
            {
                var floppyFile = VirtualFloppyFile.OpenFloppyFile(fileName);
                vm.IsValidFormat = true;
            }
            catch (Exception)
            {
                vm.IsValidFormat = false;
            }
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName"></param>
        public override void SaveFile(string fileName)
        {
            File.Copy(_loadPath, fileName);
        }
    }
}