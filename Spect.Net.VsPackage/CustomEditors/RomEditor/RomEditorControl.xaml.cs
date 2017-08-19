using Spect.Net.VsPackage.Tools.Memory;

namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// This class defines the user control that displays the ROM
    /// </summary>
    public partial class RomEditorControl
    {
        private MemoryViewModel _vm;

        /// <summary>
        /// The view model that represents the ROM
        /// </summary>
        public MemoryViewModel Vm
        {
            get => _vm;
            set => DataContext = _vm = value;
        }

        public RomEditorControl()
        {
            InitializeComponent();
        }
    }
}
