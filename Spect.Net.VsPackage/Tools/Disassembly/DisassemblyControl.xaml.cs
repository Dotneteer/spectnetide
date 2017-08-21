using System.Windows.Controls;

namespace Spect.Net.VsPackage.Tools.Disassembly
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

        public ListBox DisassemblyList => DisassemblyListBox;
    }
}
