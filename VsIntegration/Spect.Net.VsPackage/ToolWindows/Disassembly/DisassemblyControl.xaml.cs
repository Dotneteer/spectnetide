using System;
using System.Windows;
using System.Windows.Controls;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyControl.xaml
    /// </summary>
    public partial class DisassemblyControl
    {
        public DisassemblyControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The list of disassembly items
        /// </summary>
        public ListBox DisassemblyList => DisassemblyListBox;

        /// <summary>
        /// Retrieves the address of the top item, provided, there's any
        /// </summary>
        public ushort? TopAddress { get; private set; }

        /// <summary>
        /// Raised when the TopAddress property changes
        /// </summary>
        public event EventHandler<TopAddressChangedEventArgs> TopAddressChanged;

        /// <summary>
        /// We keep track of the top address
        /// </summary>
        private void OnLayoutUpdated(object sender, System.EventArgs e)
        {
            var stack = DisassemblyList.GetInnerStackPanel();
            if (stack == null) return;
            if (stack.Children.Count > 0)
            {
                var topItem = (stack.Children[0] as FrameworkElement)?.DataContext as DisassemblyItemViewModel;
                SetNewTopAddress(topItem?.Item.Address);
            }
            else
            {
                SetNewTopAddress(null);
            }
        }

        /// <summary>
        /// Detects the changes of the TopAddress property
        /// </summary>
        /// <param name="newAddress">The new address value</param>
        private void SetNewTopAddress(ushort? newAddress)
        {
            if (TopAddress == newAddress) return;
            var oldAddress = TopAddress;
            TopAddress = newAddress;
            TopAddressChanged?.Invoke(this, new TopAddressChangedEventArgs(oldAddress, newAddress));
        }
    }
}
