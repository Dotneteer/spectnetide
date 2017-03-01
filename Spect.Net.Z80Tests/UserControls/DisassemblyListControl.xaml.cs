using System.Collections.Generic;
using System.Windows;
using Spect.Net.Z80Emu.Disasm;
using Spect.Net.Z80Tests.ViewModels.Debug;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// This user control implements a Z80 disassembly list
    /// </summary>
    public partial class DisassemblyListControl
    {
        public static readonly DependencyProperty DisassemblySourceProperty = DependencyProperty.Register(
            "DisassemblySource", typeof(IList<DisassemblyItemViewModel>), typeof(DisassemblyListControl), new PropertyMetadata(default(IList<DisassemblyItemViewModel>)));

        public IList<DisassemblyItem> DisassemblySource
        {
            get { return (IList<DisassemblyItem>) GetValue(DisassemblySourceProperty); }
            set { SetValue(DisassemblySourceProperty, value); }
        }

        public DisassemblyListControl()
        {
            InitializeComponent();
        }
    }
}
