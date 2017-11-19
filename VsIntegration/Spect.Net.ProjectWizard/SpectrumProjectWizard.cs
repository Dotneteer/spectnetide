using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Models;

namespace Spect.Net.ProjectWizard
{
    /// <summary>
    /// This class implements the wizard that creates the custom project template
    /// </summary>
    public class SpectrumProjectWizard : IWizard
    {
        /// <summary>
        /// This property is used for internally enable/disable
        /// the "Create a new ZX Spectrum project" dialog
        /// </summary>
        public bool ShowSpectrumDialog => true;

        /// <summary>
        /// The Spectrum edition selected for the current project
        /// </summary>
        public SpectrumEdition SelectedEdition { get; private set; }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">
        /// The automation object being used by the template wizard.
        /// </param>
        /// <param name="replacementsDictionary">
        /// The list of standard parameters to be replaced.
        /// </param>
        /// <param name="runKind">
        /// A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind" /> 
        /// indicating the type of wizard run.
        /// </param>
        /// <param name="customParams">
        /// The custom parameters with which to perform parameter replacement in the project.
        /// </param>
        public void RunStarted(object automationObject, 
            Dictionary<string, string> replacementsDictionary, 
            WizardRunKind runKind,
            object[] customParams)
        {
            if (!ShowSpectrumDialog) return;

            var spectrumDialog = new ProjectWizardDialog();
            var result = spectrumDialog.ShowDialog();
            var selected = spectrumDialog.Vm.SelectedItem;
            if (result == null || !result.Value || selected == null)
            {
                throw new WizardCancelledException();
            }

            try
            {
                SelectedEdition = SpectrumModels.StockModels[selected.ModelKey]
                    .Editions[selected.RevisionKey];
            }
            catch
            {
                // --- If the selected edition is unavailable, stop the wizard
                throw new WizardCancelledException();
            }

            replacementsDictionary.Add("$ModelKey$", selected.ModelKey);
            replacementsDictionary.Add("$RevisionKey$", selected.RevisionKey);

            // --- Collect .rom and .disann items to add to the project
            var romInfo = SelectedEdition.Rom;
            var sb = new StringBuilder();
            for (var i = 0; i < romInfo.NumberOfRoms; i++)
            {
                var romName = romInfo.RomName + (romInfo.NumberOfRoms == 1 ? "" : $"-{i}");
                sb.Append($"<Rom Include=\"Rom\\{romName}.rom\" />\n");
                sb.Append($"<DisassAnn Include=\"Rom\\{romName}.disann\" />\n");
            }

            replacementsDictionary.Add("$romItems$", sb.ToString());
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">
        /// The project that finished generating.
        /// </param>
        public void ProjectFinishedGenerating(Project project)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">
        /// The project item that finished generating.
        /// </param>
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <returns>
        /// true if the project item should be added to the project; otherwise, false.
        /// </returns>
        /// <param name="filePath">
        /// The path to the project item.
        /// </param>
        public bool ShouldAddProjectItem(string filePath)
        {
            // --- Allow only ROMs according to the selected model
            var ext = Path.GetExtension(filePath);
            if (ext != ".rom" && ext != ".disann") return true;

            var folder = Path.GetDirectoryName(filePath);
            if (folder != "Rom") return true;

            var file = Path.GetFileNameWithoutExtension(filePath);
            var addIt = file != null && file.StartsWith(SelectedEdition.Rom.RomName);
            return addIt;
        }

        /// <summary>
        /// Runs custom wizard logic before opening an item in the template.
        /// </summary>
        /// <param name="projectItem">
        /// The project item that will be opened.
        /// </param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public void RunFinished()
        {
        }
    }
}