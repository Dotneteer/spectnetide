using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.VsPackage.Tools.Memory;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This view model represents the spectrum memory
    /// </summary>
    public class MemoryViewModel: EnhancedViewModelBase
    {
        private byte[] _memoryBuffer;

        /// <summary>
        /// The contents of the memory
        /// </summary>
        public byte[] MemoryBuffer
        {
            get => _memoryBuffer;
            set
            {
                if (!Set(ref _memoryBuffer, value)) return;
                InitMemoryLines();
            }
        }

        /// <summary>
        /// Creates the initial state of the view model
        /// </summary>
        public MemoryViewModel()
        {
            if (!IsInDesignMode) return;

            MemoryBuffer = new byte[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
            };
        }

        /// <summary>
        /// The lines (16 byte each) that represent the entire memory
        /// </summary>
        public ObservableCollection<MemoryLineViewModel> MemoryLines { get; } =
            new ObservableCollection<MemoryLineViewModel>();

        /// <summary>
        /// Initializes the memory lines with empty values
        /// </summary>
        private void InitMemoryLines()
        {
            MemoryLines.Clear();
            if (_memoryBuffer == null) return;

            for (var addr = 0x0000; addr < _memoryBuffer.Length; addr += 16)
            {
                var line = new MemoryLineViewModel(addr, _memoryBuffer.Length - 1);
                line.BindTo(MemoryBuffer);
                MemoryLines.Add(line);
            }
        }
    }
}