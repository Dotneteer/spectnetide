using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.DisannEditor
{
    /// <summary>
    /// This class represents the view model of the DisAnnEditor
    /// </summary>
    public class DisAnnEditorViewModel : EnhancedViewModelBase
    {
        private DisassemblyAnnotation _annotations;

        /// <summary>
        /// The DisassemblyAnnotations of the .disann file
        /// </summary>
        public DisassemblyAnnotation Annotations
        {
            get => _annotations;
            set => Set(ref _annotations, value);
        }
    }
}