using System.Windows;
using Microsoft.VisualStudio.TemplateWizard;

namespace Spect.Net.ProjectWizard
{
    /// <summary>
    /// Interaction logic for ProjectWizardDialog.xaml
    /// </summary>
    public partial class ProjectWizardDialog
    {
        /// <summary>
        /// The view model behind the wizard
        /// </summary>
        public SpectrumProjectWizardViewModel Vm { get; }

        public ProjectWizardDialog()
        {
            InitializeComponent();
            DataContext = Vm = new SpectrumProjectWizardViewModel();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnCreateClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
