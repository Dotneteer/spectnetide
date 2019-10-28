
using Spect.Net.VsPackage.VsxLibrary.Editor;
using System.IO;
using System.Runtime.InteropServices;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    [ComVisible(true)]
    public class TzxEditorPane : EditorPaneBase<TzxEditorFactory, TzxEditorControl>
    {
        private TapeFileViewModel _tapeFileVm;
        private string _oldFileName;

        /// <summary>
        /// Gets the file extension used by the editor.
        /// </summary>
        public override string FileExtensionUsed => TzxEditorFactory.EXTENSION;

        /// <summary>
        /// Execute loading and processing the file
        /// </summary>
        /// <param name="fileName">The name of the file to load</param>
        protected override void LoadFile(string fileName)
        {
            _oldFileName = fileName;
            _tapeFileVm = new TapeFileViewModel();
            _tapeFileVm.ReadFrom(fileName);
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = _tapeFileVm;
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="fileName">The name of the file to save</param>
        public override void SaveFile(string fileName)
        {
            if (fileName != _oldFileName)
            {
                File.Copy(_oldFileName, fileName, true);
            }
        }
    }
}
