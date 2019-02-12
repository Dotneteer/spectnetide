using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var c in _byteControls)
            {
                c.MouseEnter += OnByteMouseEnter;
                c.MouseLeave += OnByteMouseLeave;
            }
        }

        private void OnByteMouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is TextBlock tb)) return;

            if (!(DataContext is MemoryLineViewModel ml)) return;
            var addr = (int) tb.Tag;
            var regs = ml.GetAffectedRegisters(addr);
            byte.TryParse(tb.Text, NumberStyles.HexNumber, CultureInfo.CurrentUICulture, out var decValue);
            var decString = decValue.ToString();
            if (decValue != 0)
            {
                decString += "/" + (decValue - 256);
            }
            var toolTip = $"(${addr:X4}): ${tb.Text} ({decString})";
            if (regs.Count > 0)
            {
                toolTip += "\n<-- " + string.Join(", ", regs);
            }

            var symbols = ml.GetAffectedSymbols((ushort)addr);
            if (symbols.Count > 0)
            {
                toolTip += "\n<-- " + string.Join(", ", symbols);
            }
            tb.ToolTip = toolTip;
        }

        private void OnByteMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                tb.ToolTip = null;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var c in _byteControls)
            {
                c.MouseEnter -= OnByteMouseEnter;
                c.MouseLeave -= OnByteMouseLeave;
            }
        }
    }
}
