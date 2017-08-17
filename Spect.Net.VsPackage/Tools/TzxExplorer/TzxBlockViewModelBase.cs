using Spect.Net.SpectrumEmu.Mvvm;

namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// This class represents the base type of the TZX file' data blocks
    /// </summary>
    public abstract class TzxBlockViewModelBase: EnhancedViewModelBase
    {
        private int _blockId;
        private string _blockType;
        private bool _isSelected;

        /// <summary>
        /// Block identifier
        /// </summary>
        public int BlockId
        {
            get => _blockId;
            set => Set(ref _blockId, value);
        }

        /// <summary>
        /// Block type name
        /// </summary>
        public string BlockType
        {
            get => _blockType;
            set => Set(ref _blockType, value);
        }

        /// <summary>
        /// Indicates if this item is selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        protected TzxBlockViewModelBase()
        {
            BlockId = 0xFF;
            BlockType = "Unknown";
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"{BlockType} ({BlockId:X2})";
    }
}