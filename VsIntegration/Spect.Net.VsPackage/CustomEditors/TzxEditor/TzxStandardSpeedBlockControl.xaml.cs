namespace Spect.Net.VsPackage.CustomEditors.TzxEditor
{
    /// <summary>
    /// Interaction logic for TzxStandardSpeedBlockControl.xaml
    /// </summary>
    public partial class TzxStandardSpeedBlockControl
    {
        public TzxStandardSpeedBlockViewModel Vm { get; }

        public TzxStandardSpeedBlockControl()
        {
            InitializeComponent();
        }

        public TzxStandardSpeedBlockControl(TzxStandardSpeedBlockViewModel vm) : this()
        {
            DataContext = Vm = vm;
        }

        /// <summary>
        /// Switch between Data and Program view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataLabelClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Vm.IsProgramDataBlock)
            {
                Vm.ShowProgram = !Vm.ShowProgram;
            }
        }
    }
}
