using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private Dictionary<int, DisassemblyAnnotation> _annotations;

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
            if (DisassemblyAnnotation.DeserializeBankAnnotations(_contents, out var anns))
            {
                _annotations = anns;
            }
            else
            {
                _annotations = new Dictionary<int, DisassemblyAnnotation>();
                if (DisassemblyAnnotation.Deserialize(_contents, out var single))
                {
                    _annotations.Add(0, single);
                }
            }
        }

        /// <summary>
        /// Override this method to set up the editor control's view model
        /// </summary>
        protected override void OnEditorControlInitialized()
        {
            EditorControl.Vm = new DisAnnEditorViewModel
            {
                Annotations = _annotations,
                SelectedBank = _annotations.Count == 0 
                    ? null : _annotations.OrderBy(k => k.Key).First().Value,
                SelectedBankIndex = _annotations.Count == 0 
                    ? 0 : _annotations.OrderBy(k => k.Key).First().Key
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