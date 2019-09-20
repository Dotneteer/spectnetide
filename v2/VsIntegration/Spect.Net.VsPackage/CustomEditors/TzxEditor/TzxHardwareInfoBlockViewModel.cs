using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using System.Collections.ObjectModel;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// This view model represents the hardware info TZX block
    /// </summary>
    public class TzxHardwareInfoBlockViewModel : TapeBlockViewModelBase
    {
        private ObservableCollection<TzxHwBlockItemViewModel> _items;

        public ObservableCollection<TzxHwBlockItemViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public TzxHardwareInfoBlockViewModel()
        {
            BlockId = 0x33;
            BlockType = "Hardware information";
            Items = new ObservableCollection<TzxHwBlockItemViewModel>();
        }

        public void FromDataBlock(TzxHardwareInfoDataBlock dataBlock)
        {
            foreach (var item in dataBlock.HwInfo)
            {
                Items.Add(new TzxHwBlockItemViewModel(item));
            }
        }
    }
}
