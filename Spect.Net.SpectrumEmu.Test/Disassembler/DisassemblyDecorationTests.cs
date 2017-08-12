using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class DisassemblyDecorationTests
    {
        [TestMethod]
        public void ConstructorWorksAsExpected()
        {
            // --- Act
            var dc = new DisassemblyDecoration();

            // --- Assert
            dc.Labels.Count.ShouldBe(0);
            dc.Comments.Count.ShouldBe(0);
            dc.PrefixComments.Count.ShouldBe(0);
            dc.Literals.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelDoesNotSaveInvalidLabel()
        {
            // --- Arrange
            const string LABEL = "My$$Label$$";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(false);
            dc.Labels.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelTruncatesTooLongLabel()
        {
            // --- Arrange
            const string LABEL = "Label012345678901234567890123456789";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL.Substring(0, DisassembyAnnotations.MAX_LABEL_LENGTH));
        }

        [TestMethod]
        public void CreateCustomLabelWorksAsExpected()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL);
        }

        [TestMethod]
        public void CreateCustomLabelWorksWithMultipleLabels()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.SetLabel(0x1000, LABEL);
            var result2 = dc.SetLabel(0x2000, LABEL2);

            // --- Assert
            result1.ShouldBe(true);
            dc.Labels.Count.ShouldBe(2);
            dc.Labels[0x1000].ShouldBe(LABEL);
            result2.ShouldBe(true);
            dc.Labels[0x2000].ShouldBe(LABEL2);
        }

        [TestMethod]
        public void CreateCustomLabelOverwritesExistingLabel()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();
            dc.SetLabel(0x1000, LABEL);

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL2);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL2);
        }

        [TestMethod]
        public void CreateCustomLabelRemovesNullLabel()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyDecoration();
            dc.SetLabel(0x1000, LABEL);

            // --- Act
            var result = dc.SetLabel(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelRemovesWhitespaceLabel()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyDecoration();
            dc.SetLabel(0x1000, LABEL);

            // --- Act
            var result = dc.SetLabel(0x1000, "   \t\t \r ");

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomLabelHandlesNoRemove()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyDecoration();
            dc.SetLabel(0x1000, LABEL);

            // --- Act
            var result = dc.SetLabel(0x2000, null);

            // --- Assert
            result.ShouldBe(false);
            dc.Labels.Count.ShouldBe(1);
        }

        [TestMethod]
        public void CreateCustomCommentWorksAsExpected()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.SetComment(0x1000, COMMENT);

            // --- Assert
            result.ShouldBe(true);
            dc.Comments.Count.ShouldBe(1);
            dc.Comments[0x1000].ShouldBe(COMMENT);
        }

        [TestMethod]
        public void CreateCustomCommentWorksWithMultipleComments()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string COMMENT2 = "MyComment2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.SetComment(0x1000, COMMENT);
            var result2 = dc.SetComment(0x2000, COMMENT2);

            // --- Assert
            result1.ShouldBe(true);
            dc.Comments.Count.ShouldBe(2);
            dc.Comments[0x1000].ShouldBe(COMMENT);
            result2.ShouldBe(true);
            dc.Comments[0x2000].ShouldBe(COMMENT2);
        }

        [TestMethod]
        public void CreateCustomCommentOverwritesExistingComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string COMMENT2 = "MyComment2";
            var dc = new DisassemblyDecoration();
            dc.SetComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetComment(0x1000, COMMENT2);

            // --- Assert
            result.ShouldBe(true);
            dc.Comments.Count.ShouldBe(1);
            dc.Comments[0x1000].ShouldBe(COMMENT2);
        }

        [TestMethod]
        public void CreateCustomCommentRemovesNullComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();
            dc.SetComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetComment(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            dc.Comments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomCommentRemovesWhitespaceComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();
            dc.SetComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetComment(0x1000, "   \t\t \r ");

            // --- Assert
            result.ShouldBe(true);
            dc.Comments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomCommentHandlesNoRemove()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();
            dc.SetComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetComment(0x2000, null);

            // --- Assert
            result.ShouldBe(false);
            dc.Comments.Count.ShouldBe(1);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentWorksAsExpected()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.SetPrefixComment(0x1000, COMMENT);

            // --- Assert
            result.ShouldBe(true);
            dc.PrefixComments.Count.ShouldBe(1);
            dc.PrefixComments[0x1000].ShouldBe(COMMENT);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentWorksWithMultipleComments()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string COMMENT2 = "MyComment2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.SetPrefixComment(0x1000, COMMENT);
            var result2 = dc.SetPrefixComment(0x2000, COMMENT2);

            // --- Assert
            result1.ShouldBe(true);
            dc.PrefixComments.Count.ShouldBe(2);
            dc.PrefixComments[0x1000].ShouldBe(COMMENT);
            result2.ShouldBe(true);
            dc.PrefixComments[0x2000].ShouldBe(COMMENT2);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentOverwritesExistingComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            const string COMMENT2 = "MyComment2";
            var dc = new DisassemblyDecoration();
            dc.SetPrefixComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetPrefixComment(0x1000, COMMENT2);

            // --- Assert
            result.ShouldBe(true);
            dc.PrefixComments.Count.ShouldBe(1);
            dc.PrefixComments[0x1000].ShouldBe(COMMENT2);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentRemovesNullComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();
            dc.SetPrefixComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetPrefixComment(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            dc.PrefixComments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentRemovesWhitespaceComment()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();
            dc.SetPrefixComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetPrefixComment(0x1000, "   \t\t \r ");

            // --- Assert
            result.ShouldBe(true);
            dc.PrefixComments.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateCustomPrefixCommentHandlesNoRemove()
        {
            // --- Arrange
            const string COMMENT = "MyComment";
            var dc = new DisassemblyDecoration();
            dc.SetPrefixComment(0x1000, COMMENT);

            // --- Act
            var result = dc.SetPrefixComment(0x2000, null);

            // --- Assert
            result.ShouldBe(false);
            dc.PrefixComments.Count.ShouldBe(1);
        }
    }
}
