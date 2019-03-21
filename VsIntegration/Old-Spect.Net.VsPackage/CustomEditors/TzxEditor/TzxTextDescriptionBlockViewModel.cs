using System.ComponentModel;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This class implements the view model for the text description TZX data block
    /// </summary>
    public class TzxTextDescriptionBlockViewModel : TapeBlockViewModelBase
    {
        private string _text;

        [Description("Text description attached to the TZX file")]
        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public TzxTextDescriptionBlockViewModel()
        {
            BlockId = 0x30;
            BlockType = "Text Description";
        }
    }
}