using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.ProjectWizard;

namespace Spect.Net.VsPackage.Test.ProjectWizard
{
    [TestClass]
    public class ProjectWizardTest
    {
        [TestMethod]
        public void ProjectWizardConstructionWorks()
        {
            // --- Act
            var wizard = new SpectrumProjectWizard();
            Console.WriteLine(wizard.GetType().FullName);
            Console.WriteLine(wizard.GetType().Assembly.FullName);

            // --- Assert
            wizard.ShouldNotBeNull();
        }
    }
}
