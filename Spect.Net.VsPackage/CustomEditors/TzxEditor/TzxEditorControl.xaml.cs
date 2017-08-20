using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.VsPackage.Tools.TzxExplorer;

namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TzxEditorControl.xaml
    /// </summary>
    public partial class TzxEditorControl
    {
        private TzxViewModel _vm;

        /// <summary>
        /// The view model behind this control
        /// </summary>
        public TzxViewModel Vm
        {
            get => _vm;
            set
            {
                DataContext = _vm = value;
                var defaultItem = Vm.Blocks[0];
                defaultItem.IsSelected = true;
                BlockList.SelectedItem = defaultItem;
            }
        }

        public TzxEditorControl()
        {
            InitializeComponent();
            Loaded += (s, e) => Messenger.Default.Register<TzxBlockSelectedMessage>(this, OnBlockSelected);
            Unloaded += (s, e) => Messenger.Default.Unregister<TzxBlockSelectedMessage>(this);
        }

        private void OnBlockSelected(TzxBlockSelectedMessage msg)
        {
            if (msg.Block == null) return;
            Control control;
            switch (msg.Block.BlockId)
            {
                case 0x00:
                    control = new TzxHeaderBlockControl((TzxHeaderBlockViewModel)msg.Block);
                    break;

                case 0x10:
                    control = new TzxStandardSpeedBlockControl((TzxStandardSpeedBlockViewModel)msg.Block);
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
