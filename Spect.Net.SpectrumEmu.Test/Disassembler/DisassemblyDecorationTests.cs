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
        public void CreateLabelDoesNotSaveInvalidLabel()
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
        public void CreateLabelTruncatesTooLongLabel()
        {
            // --- Arrange
            const string LABEL = "Label012345678901234567890123456789";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL.Substring(0, DisassemblyDecoration.MAX_LABEL_LENGTH));
        }

        [TestMethod]
        public void CreateLabelWorksAsExpected()
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
        public void CreateLabelWorksWithMultipleLabels()
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
        public void CreateLabelOverwritesExistingLabel()
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
        public void CreateLabelRemovesNullLabel()
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
        public void CreateLabelRemovesWhitespaceLabel()
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
        public void CreateLabelHandlesNoRemove()
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
        public void CreateCommentWorksAsExpected()
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
        public void CreateCommentWorksWithMultipleComments()
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
        public void CreateCommentOverwritesExistingComment()
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
        public void CreateCommentRemovesNullComment()
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
        public void CreateCommentRemovesWhitespaceComment()
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
        public void CreateCommentHandlesNoRemove()
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
        public void CreatePrefixCommentWorksAsExpected()
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
        public void CreatePrefixCommentWorksWithMultipleComments()
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
        public void CreatePrefixCommentOverwritesExistingComment()
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
        public void CreatePrefixCommentRemovesNullComment()
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
        public void CreatePrefixCommentRemovesWhitespaceComment()
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
        public void CreatePrefixCommentHandlesNoRemove()
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

        [TestMethod]
        public void AddLiteralWorksAsExpected()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result = dc.AddLiteral(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Literals.Count.ShouldBe(1);
            var literals = dc.Literals[0x1000];
            literals.Count.ShouldBe(1);
            literals.ShouldContain(l => l == LABEL);
        }

        [TestMethod]
        public void AddLiteralWorksWithMultipleLiterals()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.AddLiteral(0x1000, LABEL);
            var result2 = dc.AddLiteral(0x2000, LABEL2);

            // --- Assert
            result1.ShouldBe(true);
            dc.Literals.Count.ShouldBe(2);
            var literals = dc.Literals[0x1000];
            literals.Count.ShouldBe(1);
            literals.ShouldContain(l => l == LABEL);
            result2.ShouldBe(true);
            literals = dc.Literals[0x2000];
            literals.Count.ShouldBe(1);
            literals.ShouldContain(l => l == LABEL2);
        }

        [TestMethod]
        public void AddLiteralWorksWithMultipleNames()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.AddLiteral(0x1000, LABEL);
            var result2 = dc.AddLiteral(0x1000, LABEL2);

            // --- Assert
            result1.ShouldBe(true);
            result2.ShouldBe(true);
            dc.Literals.Count.ShouldBe(1);
            var literals = dc.Literals[0x1000];
            literals.Count.ShouldBe(2);
            literals.ShouldContain(l => l == LABEL);
            literals.ShouldContain(l => l == LABEL2);
        }

        [TestMethod]
        public void AddLiteralStoresDistinctNames()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.AddLiteral(0x1000, LABEL);
            var result2 = dc.AddLiteral(0x1000, LABEL2);
            var result3 = dc.AddLiteral(0x1000, LABEL);

            // --- Assert
            result1.ShouldBe(true);
            result2.ShouldBe(true);
            result3.ShouldBe(false);
            dc.Literals.Count.ShouldBe(1);
            var literals = dc.Literals[0x1000];
            literals.Count.ShouldBe(2);
            literals.ShouldContain(l => l == LABEL);
            literals.ShouldContain(l => l == LABEL2);
        }

        [TestMethod]
        public void AddLiteralDoesNotAllowDifferentKeysForTheSameName()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();

            // --- Act
            var result1 = dc.AddLiteral(0x1000, LABEL);
            var result2 = dc.AddLiteral(0x1000, LABEL2);
            var result3 = dc.AddLiteral(0x2000, LABEL);

            // --- Assert
            result1.ShouldBe(true);
            result2.ShouldBe(true);
            result3.ShouldBe(false);
            dc.Literals.Count.ShouldBe(1);
            var literals = dc.Literals[0x1000];
            literals.Count.ShouldBe(2);
            literals.ShouldContain(l => l == LABEL);
            literals.ShouldContain(l => l == LABEL2);
        }

        [TestMethod]
        public void RemoveLiteralWorksAsExpected()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyDecoration();
            dc.AddLiteral(0x1000, LABEL);

            // --- Act
            var result = dc.RemoveLiteral(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Literals.Count.ShouldBe(0);
        }

        [TestMethod]
        public void RemoveLiteralKeepsUntouchedNames()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();
            dc.AddLiteral(0x1000, LABEL);
            dc.AddLiteral(0x1000, LABEL2);

            // --- Act
            var result1 = dc.RemoveLiteral(0x1000, LABEL);

            // --- Assert
            result1.ShouldBe(true);
            dc.Literals.Count.ShouldBe(1);
            var literals = dc.Literals[0x1000];
            literals.Count.ShouldBe(1);
            literals.ShouldContain(l => l == LABEL2);
        }

        [TestMethod]
        public void RemoveLiteralKeepsUntouchedKeys()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyDecoration();
            dc.AddLiteral(0x1000, LABEL);
            dc.AddLiteral(0x2000, LABEL2);

            // --- Act
            var result1 = dc.RemoveLiteral(0x1000, LABEL);

            // --- Assert
            result1.ShouldBe(true);
            dc.Literals.Count.ShouldBe(1);
            var literals = dc.Literals[0x2000];
            literals.Count.ShouldBe(1);
            literals.ShouldContain(l => l == LABEL2);
        }
    }
}
