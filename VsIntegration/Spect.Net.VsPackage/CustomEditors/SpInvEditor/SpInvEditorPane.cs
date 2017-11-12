using System.IO;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.SpInvEditor
{
    /// <summary>
    /// Editor pane class for the Spectrum Inventory viewer
    /// </summary>
    public class SpInvEditorPane : EditorPaneBase<SpectNetPackage, SpInvEditorFactory, SpInvEditorControl>
    {
        private string _contents;

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public override string FileExtensionUsed => SpInvEditorFactory.EXTENSION;

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName">The name of the file to load</param>
        protected override void LoadFile(string fileName)
        {
            // --- Read the .disann file
            _contents = File.ReadAllText(fileName);
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = new SpInvEditorViewModel();
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