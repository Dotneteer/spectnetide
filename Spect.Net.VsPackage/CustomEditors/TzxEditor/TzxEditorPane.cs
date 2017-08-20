using System.IO;
using System.Runtime.InteropServices;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    [ComVisible(true)]
    public class TzxEditorPane: EditorPaneBase<SpectNetPackage, TzxEditorFactory, TzxEditorControl>
    {
        private TzxViewModel _tzxVm;
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
            _tzxVm = new TzxViewModel();
            _tzxVm.ReadFrom(fileName);
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = _tzxVm;
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