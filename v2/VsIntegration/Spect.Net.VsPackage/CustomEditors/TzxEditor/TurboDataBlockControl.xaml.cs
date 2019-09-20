namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TurboDataBlockControl.xaml
    /// </summary>
    public partial class TurboDataBlockControl
    {
        public TzxTurboSpeedBlockViewModel Vm { get; }

        public TurboDataBlockControl()
        {
            InitializeComponent();
        }

        public TurboDataBlockControl(TzxTurboSpeedBlockViewModel vm) : this()
        {
            DataContext = Vm = vm;
        }
    }
}
