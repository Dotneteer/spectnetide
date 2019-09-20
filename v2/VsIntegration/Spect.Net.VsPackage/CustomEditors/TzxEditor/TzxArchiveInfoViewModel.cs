using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using System.Collections.ObjectModel;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    public class TzxArchiveInfoViewModel : TapeBlockViewModelBase
    {
        private ObservableCollection<TzxArchiveTextItemViewModel> _items;

        public ObservableCollection<TzxArchiveTextItemViewModel> Items
        {
            get => _items;
            set => Set(ref _items, value);
        }

        public TzxArchiveInfoViewModel()
        {
            BlockId = 0x32;
            BlockType = "Archive Info";
            Items = new ObservableCollection<TzxArchiveTextItemViewModel>();
        }

        public void FromDataBlock(TzxArchiveInfoDataBlock block)
        {
            foreach (var text in block.TextStrings)
            {
                var item = new TzxArchiveTextItemViewModel { Text = text.Text };
                item.SetType(text.Type);
                Items.Add(item);
            }
        }
    }
}
