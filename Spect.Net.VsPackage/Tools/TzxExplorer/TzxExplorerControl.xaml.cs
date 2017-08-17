using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.VsPackage.Tools.TzxExplorer
{
    /// <summary>
    /// Interaction logic for TzxExplorerControl.xaml
    /// </summary>
    public partial class TzxExplorerControl
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public TzxExplorerViewModel Vm { get; }

        public TzxExplorerControl()
        {
            InitializeComponent();
            DataContext = Vm = new TzxExplorerViewModel();
            Messenger.Default.Register<TzxBlockSelectedMessage>(this, OnBlockSelected);
            Messenger.Default.Register<TzxFileLoadedMessage>(this, OnFileLoaded);
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

        /// <summary>
        /// It is time to set the current block to the header
        /// </summary>
        /// <param name="obj"></param>
        private void OnFileLoaded(TzxFileLoadedMessage obj)
        {
            var defaultItem = Vm.Blocks[0];
            defaultItem.IsSelected = true;
            BlockList.SelectedItem = defaultItem;
        }

        /// <summary>
        /// Displays the selected TZX block information
        /// </summary>
        /// <remarks>
        /// Instead of handling this event, the event should be bound to the view model's 
        /// BlockSelectedCommand.
        /// </remarks>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vm.BlockSelectedCommand.Execute(null);
        }
    }
}
