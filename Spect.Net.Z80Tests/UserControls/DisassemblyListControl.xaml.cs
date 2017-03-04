using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        public IList<DisassemblyItemViewModel> DisassemblySource
        {
            get { return (IList<DisassemblyItemViewModel>) GetValue(DisassemblySourceProperty); }
            set { SetValue(DisassemblySourceProperty, value); }
        }

        public SpectrumDebugViewModel Vm { get; set; }


        public DisassemblyListControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Vm = DataContext as SpectrumDebugViewModel;
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address"></param>
        public void ScrollTo(ushort address)
        {
            var item = DisassemblySource.FirstOrDefault(i => i.Item.Address == address);
            if (item != null)
            {
                DisassemblyList.ScrollIntoView(item);
            }
        }
    }
}
