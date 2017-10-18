using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TzxEditorControl.xaml
    /// </summary>
    public partial class TzxEditorControl
    {
        private TapeFileViewModel _vm;
        private bool _firstLoad = true;

        /// <summary>
        /// The view model behind this control
        /// </summary>
        public TapeFileViewModel Vm
        {
            get => _vm;
            set
            {
                DataContext = _vm = value;
                SelectDefaultItem();
            }
        }

        public TzxEditorControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                Messenger.Default.Register<TzxBlockSelectedMessage>(this, OnBlockSelected);
                if (_firstLoad)
                {
                    SelectDefaultItem();
                }
                _firstLoad = false;
            };
            Unloaded += (s, e) => Messenger.Default.Unregister<TzxBlockSelectedMessage>(this);
        }

        private void SelectDefaultItem()
        {
            if (Vm?.Blocks == null || Vm.Blocks.Count == 0) return;
            var defaultItem = Vm.Blocks[0];
            defaultItem.IsSelected = true;
            BlockList.SelectedItem = defaultItem;
            Vm.BlockSelectedCommand.Execute(null);
        }

        private void OnBlockSelected(TzxBlockSelectedMessage msg)
        {
            if (msg.Sender != _vm || msg.Block == null) return;
            Control control;
            switch (msg.Block.BlockId)
            {
                case 0x00:
                    control = new TzxHeaderBlockControl((TzxHeaderBlockViewModel)msg.Block);
                    break;

                case 0x10:
                    control = new StandardDataBlockControl((TzxStandardSpeedBlockViewModel)msg.Block);
                    break;

                case 0x30:
                    control = new TzxTextDescriptionControl((TzxTextDescriptionBlockViewModel)msg.Block);
                    break;

                case 0x32:
                    control = new TzxArchiveInfoControl((TzxArchiveInfoViewModel)msg.Block);
                    break;

                default:
                    control = new TzxOtherBlockControl();
                    break;
            }
            TzxBlockContainer.Child = control;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vm.BlockSelectedCommand.Execute(null);
        }
    }
}
