using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.BasicList
{
    public class BasicLineViewModel : EnhancedViewModelBase
    {
        private int _lineNo;
        private int _length;
        private string _text;

        /// <summary>
        /// Line number
        /// </summary>
        public int LineNo
        {
            get => _lineNo;
            set => Set(ref _lineNo, value);
        }

        public int Length
        {
            get => _length;
            set => Set(ref _length, value);
        }

        /// <summary>
        /// BASIC line text
        /// </summary>
        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }
    }
}
