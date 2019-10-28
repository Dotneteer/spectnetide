using System.Windows.Controls;

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
                if (_vm != null)
                {
                    _vm.TzxBlockSelected -= VmOnTzxBlockSelected;
                }
                DataContext = _vm = value;
                SelectDefaultItem();
                if (_vm != null)
                {
                    _vm.TzxBlockSelected += VmOnTzxBlockSelected;
                }
            }
        }

        public TzxEditorControl()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (_firstLoad)
                {
                    SelectDefaultItem();
                }
                _firstLoad = false;
            };
        }

        private void SelectDefaultItem()
        {
            if (Vm?.Blocks == null || Vm.Blocks.Count == 0) return;
            var defaultItem = Vm.Blocks[0];
            defaultItem.IsSelected = true;
            BlockList.SelectedItem = defaultItem;
            Vm.BlockSelectedCommand.Execute(null);
        }

        private void VmOnTzxBlockSelected(object sender, TzxBlockSelectedEventArgs args)
        {
            if (sender != _vm || args.Block == null) return;
            Control control;
            switch (args.Block.BlockId)
            {
                case 0x00:
                    control = new TzxHeaderBlockControl((TzxHeaderBlockViewModel)args.Block);
                    break;

                case 0x10:
                    control = new StandardDataBlockControl((TzxStandardSpeedBlockViewModel)args.Block);
                    break;

                case 0x11:
                    control = new TurboDataBlockControl((TzxTurboSpeedBlockViewModel)args.Block);
                    break;

                case 0x30:
                    control = new TzxTextDescriptionControl((TzxTextDescriptionBlockViewModel)args.Block);
                    break;

                case 0x32:
                    control = new TzxArchiveInfoControl((TzxArchiveInfoViewModel)args.Block);
                    break;

                case 0x33:
                    control = new TzxHardwareInfoControl((TzxHardwareInfoBlockViewModel)args.Block);
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
