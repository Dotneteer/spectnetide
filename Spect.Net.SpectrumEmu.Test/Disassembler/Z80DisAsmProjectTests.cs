using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class Z80DisAsmProjectTests
    {
        [TestMethod]
        public void ConstructionCreatesAnEmptyProject()
        {
            // --- Act
            var project = new Z80DisassembyProject();

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
            var project = new Z80DisassembyProject();
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
            var project = new Z80DisassembyProject();
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
            var project = new Z80DisassembyProject();
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
            var project = new Z80DisassembyProject();

            // --- Act
            project.SetCustomLabel(0x2000, LABEL_NAME);

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe(LABEL_NAME.Substring(0, 16));
        }

        [TestMethod]
        public void SetCustomLabelDoesNotAcceptNull()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();

            // --- Act
            project.SetCustomLabel(0x2000, null);

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe("L2000");
        }

        [TestMethod]
        public void SetCustomLabelDoesNotAcceptInvalidName()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();

            // --- Act
            project.SetCustomLabel(0x2000, "$/%asas55__!");

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe("L2000");
        }

        [TestMethod]
        public void SetCustomLabelDoesNotAcceptKeyword()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();

            // --- Act
            project.SetCustomLabel(0x2000, "ldir");

            // --- Assert
            project.GetLabelNameByAddress(0x2000).ShouldBe("L2000");
        }

        [TestMethod]
        public void GetCommentByAddressWorksAsExpected()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
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
            var project = new Z80DisassembyProject();

            // --- Act
            project.SetComment(0x1000, null);

            // --- Assert
            project.GetCommentByAddress(0x1000).ShouldBeNull();
        }

        [TestMethod]
        public void SetCommentDoesNotAcceptWhitespaceOnly()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();

            // --- Act
            project.SetComment(0x1000, "   \t\t \n  ");

            // --- Assert
            project.GetCommentByAddress(0x1000).ShouldBeNull();
        }

        [TestMethod]
        public void RemoveDataSectionWorksAsExpected()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
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

        [TestMethod]
        public void AddDataSectionConvertsWordToByte()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x0002, DataSectionType.Word));
            var section = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x0000);

            // --- Assert
            project.DataSections.Count.ShouldBe(1);
            section.FromAddr.ShouldBe((ushort)0x0000);
            section.ToAddr.ShouldBe((ushort)0x0002);
            section.SectionType.ShouldBe(DataSectionType.Byte);
        }

        [TestMethod]
        public void AddDataSectionWorksWithNoOverlap()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x0FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x2000, 0x2FFF, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x0000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x2000);

            // --- Assert
            project.DataSections.Count.ShouldBe(2);
            section1.FromAddr.ShouldBe((ushort)0x0000);
            section1.ToAddr.ShouldBe((ushort)0x0FFF);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x2000);
            section2.ToAddr.ShouldBe((ushort)0x2FFF);
            section2.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionRemovesEntirelyOverlappedSection1()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x0FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x2FFF, DataSectionType.Word));
            var section = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x0000);

            // --- Assert
            project.DataSections.Count.ShouldBe(1);
            section.ShouldNotBeNull();

            section.FromAddr.ShouldBe((ushort)0x0000);
            section.ToAddr.ShouldBe((ushort)0x2FFF);
            section.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionRemovesEntirelyOverlappedSection2()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x1FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x2FFF, DataSectionType.Word));
            var section = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x0000);

            // --- Assert
            project.DataSections.Count.ShouldBe(1);
            section.ShouldNotBeNull();

            section.FromAddr.ShouldBe((ushort)0x0000);
            section.ToAddr.ShouldBe((ushort)0x2FFF);
            section.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionRemovesEntirelyOverlappedSection3()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x2FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x2FFF, DataSectionType.Word));
            var section = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x0000);

            // --- Assert
            project.DataSections.Count.ShouldBe(1);
            section.ShouldNotBeNull();

            section.FromAddr.ShouldBe((ushort)0x0000);
            section.ToAddr.ShouldBe((ushort)0x2FFF);
            section.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionAdjustsOverlappedSections1()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x1FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x0000, 0x1100, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x0000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1101);

            // --- Assert
            project.DataSections.Count.ShouldBe(2);

            section1.FromAddr.ShouldBe((ushort)0x0000);
            section1.ToAddr.ShouldBe((ushort)0x1100);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x1101);
            section2.ToAddr.ShouldBe((ushort)0x1FFF);
            section2.SectionType.ShouldBe(DataSectionType.Byte);
        }

        [TestMethod]
        public void AddDataSectionAdjustsOverlappedSections2()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x1FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x1200, 0x20FF, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1200);

            // --- Assert
            project.DataSections.Count.ShouldBe(2);

            section1.FromAddr.ShouldBe((ushort)0x1000);
            section1.ToAddr.ShouldBe((ushort)0x11FF);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x1200);
            section2.ToAddr.ShouldBe((ushort)0x20FF);
            section2.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionAdjustsOverlappedSections3()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x1FFF, DataSectionType.Byte));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x1200, 0x18FF, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1200);
            var section3 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1900);

            // --- Assert
            project.DataSections.Count.ShouldBe(3);

            section1.FromAddr.ShouldBe((ushort)0x1000);
            section1.ToAddr.ShouldBe((ushort)0x11FF);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x1200);
            section2.ToAddr.ShouldBe((ushort)0x18FF);
            section2.SectionType.ShouldBe(DataSectionType.Word);

            section3.FromAddr.ShouldBe((ushort)0x1900);
            section3.ToAddr.ShouldBe((ushort)0x1FFF);
            section3.SectionType.ShouldBe(DataSectionType.Byte);
        }

        [TestMethod]
        public void AddDataSectionAdjustsOverlappedSections4()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x1FFF, DataSectionType.Word));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x1201, 0x18FF, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1201);
            var section3 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1900);

            // --- Assert
            project.DataSections.Count.ShouldBe(3);

            section1.FromAddr.ShouldBe((ushort)0x1000);
            section1.ToAddr.ShouldBe((ushort)0x1200);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x1201);
            section2.ToAddr.ShouldBe((ushort)0x18FF);
            section2.SectionType.ShouldBe(DataSectionType.Byte);

            section3.FromAddr.ShouldBe((ushort)0x1900);
            section3.ToAddr.ShouldBe((ushort)0x1FFF);
            section3.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionAdjustsMultipleOverlappedSections1()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x13FF, DataSectionType.Word));
            project.AddDataSection(new DisassemblyDataSection(0x1600, 0x1FFF, DataSectionType.Word));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x1201, 0x18FF, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1201);
            var section3 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1900);

            // --- Assert
            project.DataSections.Count.ShouldBe(3);

            section1.FromAddr.ShouldBe((ushort)0x1000);
            section1.ToAddr.ShouldBe((ushort)0x1200);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x1201);
            section2.ToAddr.ShouldBe((ushort)0x18FF);
            section2.SectionType.ShouldBe(DataSectionType.Byte);

            section3.FromAddr.ShouldBe((ushort)0x1900);
            section3.ToAddr.ShouldBe((ushort)0x1FFF);
            section3.SectionType.ShouldBe(DataSectionType.Word);
        }

        [TestMethod]
        public void AddDataSectionAdjustsMultipleOverlappedSections2()
        {
            // --- Arrange
            var project = new Z80DisassembyProject();
            project.AddDataSection(new DisassemblyDataSection(0x1000, 0x13FF, DataSectionType.Word));
            project.AddDataSection(new DisassemblyDataSection(0x1600, 0x17FF, DataSectionType.Word));

            // --- Act
            project.AddDataSection(new DisassemblyDataSection(0x1201, 0x18FF, DataSectionType.Word));
            var section1 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1000);
            var section2 = project.DataSections.FirstOrDefault(ds => ds.FromAddr == 0x1201);

            // --- Assert
            project.DataSections.Count.ShouldBe(2);

            section1.FromAddr.ShouldBe((ushort)0x1000);
            section1.ToAddr.ShouldBe((ushort)0x1200);
            section1.SectionType.ShouldBe(DataSectionType.Byte);

            section2.FromAddr.ShouldBe((ushort)0x1201);
            section2.ToAddr.ShouldBe((ushort)0x18FF);
            section2.SectionType.ShouldBe(DataSectionType.Byte);
        }
    }
}
