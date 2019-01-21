using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// Interaction logic for MemoryLineControl.xaml
    /// </summary>
    public partial class MemoryLineControl
    {
        private readonly List<TextBlock> _byteControls = new List<TextBlock>();

        public MemoryLineControl()
        {
            InitializeComponent();
            _byteControls.Add(Byte0);
            _byteControls.Add(Byte1);
            _byteControls.Add(Byte2);
            _byteControls.Add(Byte3);
            _byteControls.Add(Byte4);
            _byteControls.Add(Byte5);
            _byteControls.Add(Byte6);
            _byteControls.Add(Byte7);
            _byteControls.Add(Byte8);
            _byteControls.Add(Byte9);
            _byteControls.Add(ByteA);
            _byteControls.Add(ByteB);
            _byteControls.Add(ByteC);
            _byteControls.Add(ByteD);
            _byteControls.Add(ByteE);
            _byteControls.Add(ByteF);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var c in _byteControls)
            {
                c.MouseEnter += OnByteMouseEnter;
            }
        }

        private void OnByteMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                // TODO:...
            }
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var c in _byteControls)
            {
                c.MouseEnter -= OnByteMouseEnter;
            }
        }
    }
}
