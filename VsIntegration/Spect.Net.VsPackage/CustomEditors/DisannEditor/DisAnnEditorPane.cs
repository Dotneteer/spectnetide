using System.IO;
using System.Runtime.InteropServices;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.DisannEditor
{
    /// <summary>
    /// Editor pane class for the ROM viewer
    /// </summary>
    [ComVisible(true)]
    public class DisAnnEditorPane : EditorPaneBase<SpectNetPackage, DisAnnEditorFactory, DisAnnEditorControl>
    {
        private string _contents;
        private DisassemblyAnnotation _annotations;

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public override string FileExtensionUsed => DisAnnEditorFactory.EXTENSION;

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName">The name of the file to load</param>
        protected override void LoadFile(string fileName)
        {
            // --- Read the .disann file
            _contents = File.ReadAllText(fileName);
            _annotations = DisassemblyAnnotation.Deserialize(_contents);
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = new DisAnnEditorViewModel
            {
               Annotations = _annotations
            };
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName">The name of the file to save</param>
        public override void SaveFile(string fileName)
        {
            File.WriteAllText(fileName, _contents);
        }
    }
}