using System.IO;
using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;
// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// Editor pane class for the ROM viewer
    /// </summary>
    [ComVisible(true)]
    public class RomEditorPane: EditorPaneBase<SpectNetPackage, RomEditorFactory, RomEditorControl>
    {
        private byte[] _romFoleContents;

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public override string FileExtensionUsed => RomEditorFactory.EXTENSION;

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName">The name of the file to load</param>
        protected override void LoadFile(string fileName)
        {
            // --- Read the rom file
            using (var stream = new StreamReader(fileName).BaseStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                _romFoleContents = new byte[stream.Length];
                stream.Read(_romFoleContents, 0, _romFoleContents.Length);
            }
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = new MemoryViewModel
            {
                MemoryBuffer = _romFoleContents,
                AllowDisassembly = false
            };
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName">The name of the file to save</param>
        public override void SaveFile(string fileName)
        {
            using (var writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                writer.Write(_romFoleContents);
            }
        }
    }
}