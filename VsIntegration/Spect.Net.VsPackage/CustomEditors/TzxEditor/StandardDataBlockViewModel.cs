namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Represents a standard data block (in .TAP)
    /// </summary>
    public class StandardDataBlockViewModel : TzxStandardSpeedBlockViewModel
    {
        public StandardDataBlockViewModel()
        {
            BlockId = 0x10;
            BlockType = "Standard Data Block";
            if (!IsInDesignMode) return;

            HeaderType = "Header";
            IsHeaderBlock = true;
            DataType = "Program";
            Filename = "Pac-Man";
            DataBlockBytes = 100;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{BlockType}";
    }
}