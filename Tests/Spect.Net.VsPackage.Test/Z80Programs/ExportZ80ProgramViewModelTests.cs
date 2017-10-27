using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.VsPackage.Z80Programs;
// ReSharper disable UseObjectOrCollectionInitializer

namespace Spect.Net.VsPackage.Test.Z80Programs
{
    [TestClass]
    public class ExportZ80ProgramViewModelTests
    {
        [TestMethod]
        public void ConstructionWorks()
        {
            // --- Act
            var vm = new ExportZ80ProgramViewModel();

            // --- Assert
            vm.ShouldNotBeNull();
            vm.AddToProject.ShouldBeFalse();
            vm.ApplyClear.ShouldBeFalse();
            vm.AutoStart.ShouldBeFalse();
            vm.AssemblerOutput.ShouldBeNull();
            vm.AutoStart.ShouldBeFalse();
            vm.IsValid.ShouldBeFalse();
            vm.Name.ShouldBeNull();
            vm.Filename.ShouldBeNull();
            vm.Format.ShouldBe((ExportFormat)0);
            vm.SingleBlock.ShouldBeFalse();
        }

        [TestMethod]
        public void SettingNameAndFileNameMakesVmValid()
        {
            // --- Arrange
            var vm = new ExportZ80ProgramViewModel();
            
            // --- Act
            vm.Name = "MyCode";
            vm.Filename = "MyCode.tzx";

            // --- Assert
            vm.IsValid.ShouldBeTrue();
        }

        [TestMethod]
        public void EmptyNameMakesVmInvalid()
        {
            // --- Arrange
            var vm = new ExportZ80ProgramViewModel();
            vm.Name = "MyCode";
            vm.Filename = "MyCode.tzx";
            var before = vm.IsValid;

            // --- Act
            vm.Name = "  \t \n  ";

            // --- Assert
            before.ShouldBeTrue();
            vm.IsValid.ShouldBeFalse();
        }

        [TestMethod]
        public void NullNameMakesVmInvalid()
        {
            // --- Arrange
            var vm = new ExportZ80ProgramViewModel();
            vm.Name = "MyCode";
            vm.Filename = "MyCode.tzx";
            var before = vm.IsValid;

            // --- Act
            vm.Name = null;

            // --- Assert
            before.ShouldBeTrue();
            vm.IsValid.ShouldBeFalse();
        }

        [TestMethod]
        public void EmptyFilenameMakesVmInvalid()
        {
            // --- Arrange
            var vm = new ExportZ80ProgramViewModel();
            vm.Name = "MyCode";
            vm.Filename = "MyCode.tzx";
            var before = vm.IsValid;

            // --- Act
            vm.Filename = "  \t \n  ";

            // --- Assert
            before.ShouldBeTrue();
            vm.IsValid.ShouldBeFalse();
        }

        [TestMethod]
        public void NullFilenameMakesVmInvalid()
        {
            // --- Arrange
            var vm = new ExportZ80ProgramViewModel();
            vm.Name = "MyCode";
            vm.Filename = "MyCode.tzx";
            var before = vm.IsValid;

            // --- Act
            vm.Filename = null;

            // --- Assert
            before.ShouldBeTrue();
            vm.IsValid.ShouldBeFalse();
        }
    }
}
