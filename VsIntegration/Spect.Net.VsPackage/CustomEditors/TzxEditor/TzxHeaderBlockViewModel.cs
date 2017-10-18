namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This class represents the header of the TZX block
    /// </summary>
    public class TzxHeaderBlockViewModel : TapeBlockViewModelBase
    {
        private int _majorVersion;
        private int _minorVersion;

        /// <summary>
        /// Major file version
        /// </summary>
        public int MajorVersion
        {
            get => _majorVersion;
            set => Set(ref _majorVersion, value);
        }

        /// <summary>
        /// Minor file version
        /// </summary>
        public int MinorVersion
        {
            get => _minorVersion;
            set => Set(ref _minorVersion, value);
        }

        public TzxHeaderBlockViewModel()
        {
            BlockId = 0x00;
            BlockType = "TZX File Header";
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{BlockType} ({MajorVersion}.{MinorVersion})";
    }
}