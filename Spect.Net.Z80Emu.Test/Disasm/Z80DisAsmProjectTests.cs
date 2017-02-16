using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Disasm;
// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.Z80Emu.Test.Disasm
{
    [TestClass]
    public class Z80DisAsmProjectTests
    {
        [TestMethod]
        public void ConstructionCreatesAnEmptyProject()
        {
            // --- Act
            var project = new Z80DisAsmProject();

            // --- Assert
            project.Z80Binary.ShouldNotBeNull();
            project.Z80Binary.Length.ShouldBe(0);
            project.StartOffset.ShouldBe((ushort)0x0000);

            project.Labels.ShouldNotBeNull();
            project.Labels.Count.ShouldBe(0);
            project.CustomLabels.ShouldNotBeNull();
            project.CustomLabels.Count.ShouldBe(0);
            project.Comments.ShouldNotBeNull();
            project.Comments.Count.ShouldBe(0);
            project.DataSections.ShouldNotBeNull();
            project.DataSections.Count.ShouldBe(0);
        }

        [TestMethod]
        public void ClearAnnotationsRemovesArtifacts()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();
            project.CollectLabel(0x1000, null);
            project.SetCustomLabel(0x2000, "MyLabel");
            project.SetComment(0x3000, "This is my comment");
            project.AddDataSection(new DisassemblyDataSection(0x0010, 0x0020, DataSectionType.Byte));

            // --- Act
            var labelsBefore = project.Labels.Count;
            var customLabelsBefore = project.CustomLabels.Count;
            var commentsBefore = project.Comments.Count;
            var sectionsBefore = project.DataSections.Count;
            project.ClearAnnotations();

            // --- Assert
            labelsBefore.ShouldBe(2);
            customLabelsBefore.ShouldBe(1);
            commentsBefore.ShouldBe(1);
            sectionsBefore.ShouldBe(1);

            project.Labels.Count.ShouldBe(0);
            project.CustomLabels.Count.ShouldBe(0);
            project.Comments.Count.ShouldBe(0);
            project.DataSections.Count.ShouldBe(0);
        }

        [TestMethod]
        public void GetLabelNameByAddressWorksAsExpected()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();
            project.CollectLabel(0x1000, null);
            project.SetCustomLabel(0x2000, "MyLabel");

            // --- Act
            var label1 = project.GetLabelNameByAddress(0x1000);
            var label2 = project.GetLabelNameByAddress(0x2000);
            var label3 = project.GetLabelNameByAddress(0x3000);

            // --- Assert
            label1.ShouldBe("L1000");
            label2.ShouldBe("MyLabel");
            label3.ShouldBe("L3000");
        }

        [TestMethod]
        public void CollectLabelWorksAsExpected()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();
            project.SetCustomLabel(0x2000, "MyLabel");

            // --- Act
            var label1 = project.CollectLabel(0x1000, 0x1001);
            var label2 = project.CollectLabel(0x1000, 0x1002);
            var label3 = project.CollectLabel(0x2000, 0x2001);

            // --- Assert
            label1.ShouldBe("L1000");
            label2.ShouldBe("L1000");
            label3.ShouldBe("MyLabel");

            var ref1 = project.Labels.Values.FirstOrDefault(l => l.Address == 0x1000).References;
            var ref2 = project.Labels.Values.FirstOrDefault(l => l.Address == 0x2000).References;

            ref1.Count.ShouldBe(2);
            ref1.ShouldContain((ushort)0x1001);
            ref1.ShouldContain((ushort)0x1002);
            ref2.Count.ShouldBe(1);
            ref2.ShouldContain((ushort)0x2001);
        }

        [TestMethod]
        public void SetCustomLabelTrimsLongLabels()
        {
            // --- Arrange
            const string LABEL_NAME = "MyVeryLongLabelName";
            var project = new Z80DisAsmProject();

            // --- Act
            project.SetCustomLabel(0x2000, LABEL_NAME);

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe(LABEL_NAME.Substring(0, 16));
        }

        [TestMethod]
        public void SetCustomLabelDoesNotAcceptNull()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();

            // --- Act
            project.SetCustomLabel(0x2000, null);

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe("L2000");
        }

        [TestMethod]
        public void SetCustomLabelDoesNotAcceptInvalidName()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();

            // --- Act
            project.SetCustomLabel(0x2000, "$/%asas55__!");

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe("L2000");
        }

        [TestMethod]
        public void GetCommentByAddressWorksAsExpected()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();
            project.SetComment(0x1000, "My comment");

            // --- Act
            var comm1 = project.GetCommentByAddress(0x1000);
            var comm2 = project.GetCommentByAddress(0x2000);

            // --- Assert
            comm1.ShouldBe("My comment");
            comm2.ShouldBeNull();
        }

        [TestMethod]
        public void SetCommentDoesNotAcceptNull()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();

            // --- Act
            project.SetComment(0x1000, null);

            // --- Assert
            project.GetCommentByAddress(0x1000).ShouldBeNull();
        }

        [TestMethod]
        public void SetCommentDoesNotAcceptWhitespaceOnly()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();

            // --- Act
            project.SetComment(0x1000, "   \t\t \n  ");

            // --- Assert
            project.GetCommentByAddress(0x1000).ShouldBeNull();
        }

        [TestMethod]
        public void RemoveDataSectionWorksAsExpected()
        {
            // --- Arrange
            var project = new Z80DisAsmProject();
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x0FFF, DataSectionType.Byte));
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x1FFF, DataSectionType.Byte));
            var otherSection = new DisassemblyDataSection(0x1000, 0x2000, DataSectionType.Byte);

            // --- Act
            var countBefore = project.DataSections.Count;
            var remove1 = project.RemoveDataSection(project.DataSections[0]);
            var remove2 = project.RemoveDataSection(otherSection);
            var countAfter = project.DataSections.Count;

            // --- Assert
            countBefore.ShouldBe(2);
            countAfter.ShouldBe(1);
            remove1.ShouldBeTrue();
            remove2.ShouldBeFalse();
        }
    }
}
