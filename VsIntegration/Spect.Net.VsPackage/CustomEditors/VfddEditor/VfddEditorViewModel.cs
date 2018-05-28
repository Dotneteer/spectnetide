using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.CustomEditors.VfddEditor
{
    /// <summary>
    /// This class represents the view model of the virtual floppy
    /// disk file editor
    /// </summary>
    public class VfddEditorViewModel: EnhancedViewModelBase
    {
        private bool _isValidFormat;
        private string _diskFormat;
        private string _sideness;
        private int _tracksPerSide;
        private int _sectorsPerTrack;
        private int _sectorSize;
        private int _reservedTracks;
        private int _blockSize;
        private int _directoryBlocks;
        private int _readWriteGapLength;
        private int _formatGapLength;

        /// <summary>
        /// Indicates if the disk file format is valid
        /// </summary>
        public bool IsValidFormat
        {
            get => _isValidFormat;
            set => Set(ref _isValidFormat, value);
        }

        /// <summary>
        /// The format of the disk
        /// </summary>
        public string DiskFormat
        {
            get => _diskFormat;
            set => Set(ref _diskFormat, value);
        }

        /// <summary>
        /// Number of disk sides
        /// </summary>
        public string Sideness
        {
            get => _sideness;
            set => Set(ref _sideness, value);
        }

        /// <summary>
        /// Number of tracks used by the disk
        /// </summary>
        public int TracksPerSide
        {
            get => _tracksPerSide;
            set => Set(ref _tracksPerSide, value);
        }

        /// <summary>
        /// Number of sectors per track
        /// </summary>
        public int SectorsPerTrack
        {
            get => _sectorsPerTrack;
            set => Set(ref _sectorsPerTrack, value);
        }

        /// <summary>
        /// Number of bytes in a sector
        /// </summary>
        public int SectorSize
        {
            get => _sectorSize;
            set => Set(ref _sectorSize, value);
        }

        /// <summary>
        /// Number of reserved tracks
        /// </summary>
        public int ReservedTracks
        {
            get => _reservedTracks;
            set => Set(ref _reservedTracks, value);
        }

        /// <summary>
        /// Size of disk blocks
        /// </summary>
        public int BlockSize
        {
            get => _blockSize;
            set => Set(ref _blockSize, value);
        }

        /// <summary>
        /// Number of directory blocks
        /// </summary>
        public int DirectoryBlocks
        {
            get => _directoryBlocks;
            set => Set(ref _directoryBlocks, value);
        }

        /// <summary>
        /// Length of the read/write gap
        /// </summary>
        public int ReadWriteGapLength
        {
            get => _readWriteGapLength;
            set => Set(ref _readWriteGapLength, value);
        }

        /// <summary>
        /// Length of format gap
        /// </summary>
        public int FormatGapLength
        {
            get => _formatGapLength;
            set => Set(ref _formatGapLength, value);
        }
    }
}