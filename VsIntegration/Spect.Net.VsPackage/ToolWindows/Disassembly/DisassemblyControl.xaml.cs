using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Spect.Net.VsPackage.Utility;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// Interaction logic for DisassemblyControl.xaml
    /// </summary>
    public partial class DisassemblyControl
    {
        private bool _isInClick;

        public DisassemblyControl()
        {
            InitializeComponent();
            _isInClick = false;
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
        /// Raised when an item is clicked
        /// </summary>
        public event EventHandler<DisassemblyItemSelectedEventArgs> ItemClicked;

        /// <summary>
        /// Raised when an item is double clicked
        /// </summary>
        public event EventHandler<DisassemblyItemSelectedEventArgs> ItemDoubleClicked;

        /// <summary>
        /// Raised when an item is triple clicked
        /// </summary>
        public event EventHandler<DisassemblyItemSelectedEventArgs> ItemTripleClicked;

        /// <summary>
        /// We keep track of the top address
        /// </summary>
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var stack = DisassemblyList.GetInnerStackPanel();
            if (stack == null) return;
            if (stack.Children.Count > 1)
            {
                var topItem = (stack.Children[1] as FrameworkElement)?.DataContext as DisassemblyItemViewModel;
                SetNewTopAddress(topItem?.Item.Address);
            }
            else if (stack.Children.Count > 0)
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

        /// <summary>
        /// Checks if a disassembly item has been double of triple clicked.
        /// </summary>
        private void OnItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DisassemblyListBox.SelectedItem is DisassemblyItemViewModel disItem)
            {
                if (e.ClickCount == 2)
                {
                    ItemDoubleClicked?.Invoke(this, new DisassemblyItemSelectedEventArgs(disItem));
                    _isInClick = true;
                }
                else if (e.ClickCount == 3)
                {
                    ItemTripleClicked?.Invoke(this, new DisassemblyItemSelectedEventArgs(disItem));
                    _isInClick = true;
                }
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DisassemblyListBox.SelectedItem is DisassemblyItemViewModel disItem)
            {
                if (e.ClickCount == 1 && !_isInClick)
                {
                    ItemClicked?.Invoke(this, new DisassemblyItemSelectedEventArgs(disItem));
                }
                _isInClick = false;
            }
        }
    }
}
