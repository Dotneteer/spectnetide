using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    public class TzxArchiveTextItemViewModel: EnhancedViewModelBase
    {
        private string _type;
        private string _text;

        public string Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public TzxArchiveTextItemViewModel()
        {
            if (IsInDesignMode)
            {
                Type = "Year of publication";
                Text = "This is a long text... This is a long text... This is a long text... This is a long text... ";
            }
        }

        public void SetType(byte type)
        {
            switch (type)
            {
                case 0x00:
                    Type = "Full Title";
                    break;
                case 0x01:
                    Type = "Publisher";
                    break;
                case 0x02:
                    Type = "Author(s)";
                    break;
                case 0x03:
                    Type = "Year of publication";
                    break;
                case 0x04:
                    Type = "Language";
                    break;
                case 0x05:
                    Type = "Software type";
                    break;
                case 0x06:
                    Type = "Price";
                    break;
                case 0x07:
                    Type = "Protection scheme";
                    break;
                case 0x08:
                    Type = "Origin";
                    break;
                case 0xFF:
                    Type = "Comments";
                    break;
                default:
                    Type = "Unknown";
                    break;
            }
            Type += ":";
        }
    }
}