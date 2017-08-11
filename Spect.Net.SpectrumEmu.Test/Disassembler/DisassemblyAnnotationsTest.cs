using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class DisassemblyAnnotationsTest
    {
        [TestMethod]
        public void ConstructorInitializesProperly()
        {
            // --- Act
            var da = new DisassembyAnnotations();

            // --- Assert
            da.CustomLabels.Count.ShouldBe(0);
            da.CustomComments.Count.ShouldBe(0);
            da.MemorySections.Count.ShouldBeGreaterThanOrEqualTo(0);
        }

        [TestMethod]
        public void ConstructorWithMemoryMapInitializesProperly()
        {
            // --- Act
            var da = new DisassembyAnnotations(new MemoryMap
            {
                new MemorySection(0x0000, 0x3CFF),
                new MemorySection(0x3D00, 0x3FFF, MemorySectionType.ByteArray),
                new MemorySection(0x4000, 0x5AFF, MemorySectionType.Skip),
            });

            // --- Assert
            da.CustomLabels.Count.ShouldBe(0);
            da.CustomComments.Count.ShouldBe(0);
            da.MemorySections.Count.ShouldBe(3);
        }

        [TestMethod]
        public void CreateCustomLabelDoesNotSaveInvalidLabel()
        {
            // --- Arrange
            const string LABEL = "My$$Label$$";
            var da = new DisassembyAnnotations();

            // --- Act
            var result = da.CreateCustomLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(false);
            da.CustomLabels.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelTruncatesTooLongLabel()
        {
            // --- Arrange
            const string LABEL = "Label012345678901234567890123456789";
            var da = new DisassembyAnnotations();

            // --- Act
            var result = da.CreateCustomLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            da.CustomLabels.Count.ShouldBe(1);
            var cl = da.CustomLabels[0x1000];
            cl.Address.ShouldBe((ushort)0x1000);
            cl.Name.ShouldBe(LABEL.Substring(0, DisassembyAnnotations.MAX_LABEL_LENGTH));
        }

        [TestMethod]
        public void CreateCustomLabelWorksAsExpected()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var da = new DisassembyAnnotations();

            // --- Act
            var result = da.CreateCustomLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            da.CustomLabels.Count.ShouldBe(1);
            var cl = da.CustomLabels[0x1000];
            cl.Address.ShouldBe((ushort)0x1000);
            cl.Name.ShouldBe(LABEL);
        }

        [TestMethod]
        public void CreateCustomLabelWorksWithMultipleLabels()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var da = new DisassembyAnnotations();

            // --- Act
            var result1 = da.CreateCustomLabel(0x1000, LABEL);
            var result2 = da.CreateCustomLabel(0x2000, LABEL2);

            // --- Assert
            result1.ShouldBe(true);
            da.CustomLabels.Count.ShouldBe(2);
            var cl = da.CustomLabels[0x1000];
            cl.Address.ShouldBe((ushort)0x1000);
            cl.Name.ShouldBe(LABEL);
            result2.ShouldBe(true);
            cl = da.CustomLabels[0x2000];
            cl.Address.ShouldBe((ushort)0x2000);
            cl.Name.ShouldBe(LABEL2);
        }

        [TestMethod]
        public void CreateCustomLabelOverwritesExistingLabel()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var da = new DisassembyAnnotations();
            da.CreateCustomLabel(0x1000, LABEL);

            // --- Act
            var result = da.CreateCustomLabel(0x1000, LABEL2);

            // --- Assert
            result.ShouldBe(true);
            da.CustomLabels.Count.ShouldBe(1);
            var cl = da.CustomLabels[0x1000];
            cl.Address.ShouldBe((ushort)0x1000);
            cl.Name.ShouldBe(LABEL2);
        }

        [TestMethod]
        public void CreateCustomLabelRemovesNullLabel()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var da = new DisassembyAnnotations();
            da.CreateCustomLabel(0x1000, LABEL);

            // --- Act
            var result = da.CreateCustomLabel(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            da.CustomLabels.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelRemovesWhitespaceLabel()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var da = new DisassembyAnnotations();
            da.CreateCustomLabel(0x1000, LABEL);

            // --- Act
            var result = da.CreateCustomLabel(0x1000, " \t\r\n  ");

            // --- Assert
            result.ShouldBe(true);
            da.CustomLabels.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelHandlesNoRemove()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var da = new DisassembyAnnotations();
            da.CreateCustomLabel(0x1000, LABEL);

            // --- Act
            var result = da.CreateCustomLabel(0x2000, null);

            // --- Assert
            result.ShouldBe(false);
            da.CustomLabels.Count.ShouldBe(1);
        }

        [TestMethod]
        public void CreateCustomCommentWorksAsExpected()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var da = new DisassembyAnnotations();

            // --- Act
            var result = da.CreateCustomComment(0x1000, COMMENT);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(1);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBe(COMMENT);
            cc.PrefixComment.ShouldBeNull();
        }

        [TestMethod]
        public void CreateCustomCommentWorksWithMultipleComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string COMMENT2 = "MyComment2";
            var da = new DisassembyAnnotations();

            // --- Act
            var result1 = da.CreateCustomComment(0x1000, COMMENT);
            var result2 = da.CreateCustomComment(0x2000, COMMENT2);

            // --- Assert
            da.CustomComments.Count.ShouldBe(2);
            result1.ShouldBe(true);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBe(COMMENT);
            cc.PrefixComment.ShouldBeNull();
            result2.ShouldBe(true);
            cc = da.CustomComments[0x2000];
            cc.Address.ShouldBe((ushort)0x2000);
            cc.Comment.ShouldBe(COMMENT2);
            cc.PrefixComment.ShouldBeNull();
        }

        [TestMethod]
        public void CreateCustomCommentOverwritesExisting()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string COMMENT2 = "MyComment2";
            var da = new DisassembyAnnotations();
            da.CreateCustomComment(0x1000, COMMENT);

            // --- Act
            var result = da.CreateCustomComment(0x1000, COMMENT2);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(1);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBe(COMMENT2);
            cc.PrefixComment.ShouldBeNull();
        }

        [TestMethod]
        public void CreateCustomCommentRemovesExistingCommentWithNull()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var da = new DisassembyAnnotations();
            da.CreateCustomComment(0x1000, COMMENT);

            // --- Act
            var result = da.CreateCustomComment(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomCommentRemovesExistingCommentWithWhitespace()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var da = new DisassembyAnnotations();
            da.CreateCustomComment(0x1000, COMMENT);

            // --- Act
            var result = da.CreateCustomComment(0x1000, "   \t \n \t ");

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomCommentRemovesExistingCommentWithNullAndKeepsPrefixComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string PREFIX_COMMENT = "MyPrefixComment";
            var da = new DisassembyAnnotations();
            da.CreateCustomComment(0x1000, COMMENT);
            da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);

            // --- Act
            var result = da.CreateCustomComment(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(1);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBeNull();
            cc.PrefixComment.ShouldBe(PREFIX_COMMENT);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentWorksAsExpected()
        {
            // --- Arrange
            const string PREFIX_COMMENT = "MyPrefixComment";
            var da = new DisassembyAnnotations();

            // --- Act
            var result = da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(1);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBeNull();
            cc.PrefixComment.ShouldBe(PREFIX_COMMENT);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentWorksWithMultipleComment()
        {
            // --- Arrange
            const string PREFIX_COMMENT = "MyPrefixComment";
            const string PREFIX_COMMENT2 = "MyPrefixComment2";
            var da = new DisassembyAnnotations();

            // --- Act
            var result1 = da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);
            var result2 = da.CreateCustomPrefixComment(0x2000, PREFIX_COMMENT2);

            // --- Assert
            da.CustomComments.Count.ShouldBe(2);
            result1.ShouldBe(true);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBeNull();
            cc.PrefixComment.ShouldBe(PREFIX_COMMENT);
            result2.ShouldBe(true);
            cc = da.CustomComments[0x2000];
            cc.Address.ShouldBe((ushort)0x2000);
            cc.Comment.ShouldBeNull();
            cc.PrefixComment.ShouldBe(PREFIX_COMMENT2);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentOverwritesExisting()
        {
            // --- Arrange
            const string PREFIX_COMMENT = "MyPrefixComment";
            const string PREFIX_COMMENT2 = "MyPrefixComment2";
            var da = new DisassembyAnnotations();
            da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);

            // --- Act
            var result = da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT2);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(1);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBeNull();
            cc.PrefixComment.ShouldBe(PREFIX_COMMENT2);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentRemovesExistingCommentWithNull()
        {
            // --- Arrange
            const string PREFIX_COMMENT = "MyPrefixComment";
            var da = new DisassembyAnnotations();
            da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);

            // --- Act
            var result = da.CreateCustomPrefixComment(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentRemovesExistingCommentWithWhitespace()
        {
            // --- Arrange
            const string PREFIX_COMMENT = "MyPrefixComment";
            var da = new DisassembyAnnotations();
            da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);

            // --- Act
            var result = da.CreateCustomPrefixComment(0x1000, "   ");

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentRemovesExistingCommentWithNullAndKeepsComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string PREFIX_COMMENT = "MyPrefixComment";
            var da = new DisassembyAnnotations();
            da.CreateCustomComment(0x1000, COMMENT);
            da.CreateCustomPrefixComment(0x1000, PREFIX_COMMENT);

            // --- Act
            var result = da.CreateCustomPrefixComment(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            da.CustomComments.Count.ShouldBe(1);
            var cc = da.CustomComments[0x1000];
            cc.Address.ShouldBe((ushort)0x1000);
            cc.Comment.ShouldBe(COMMENT);
            cc.PrefixComment.ShouldBeNull();
        }
    }
}
