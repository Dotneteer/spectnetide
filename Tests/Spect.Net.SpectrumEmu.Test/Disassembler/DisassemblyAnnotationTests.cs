using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class DisassemblyAnnotationTests
    {
        [TestMethod]
        public void ConstructorWorksAsExpected()
        {
            // --- Act
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL.Substring(0, DisassemblyAnnotation.MAX_LABEL_LENGTH));
        }

        [TestMethod]
        public void CreateLabelWorksAsExpected()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyAnnotation();

            // --- Act
            var result = dc.SetLabel(0x1000, LABEL);

            // --- Assert
            result.ShouldBe(true);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL);
        }

        [TestMethod]
        public void CreateLabelForbidsDuplicateLabelName()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            var dc = new DisassemblyAnnotation();
            dc.SetLabel(0x1000, LABEL);

            // --- Act
            var result = dc.SetLabel(0x1100, LABEL);

            // --- Assert
            result.ShouldBe(false);
            dc.Labels.Count.ShouldBe(1);
            dc.Labels[0x1000].ShouldBe(LABEL);
        }



        [TestMethod]
        public void CreateLabelWorksWithMultipleLabels()
        {
            // --- Arrange
            const string LABEL = "MyLabel";
            const string LABEL2 = "MyLabel2";
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();

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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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
            var dc = new DisassemblyAnnotation();
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

        [TestMethod]
        public void CreateReplacementWorksAsExpected()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();

            // --- Act
            var result = dc.SetLiteralReplacement(0x1000, REPLACEMENT);

            // --- Assert
            result.ShouldBe(true);
            dc.LiteralReplacements.Count.ShouldBe(1);
            dc.LiteralReplacements[0x1000].ShouldBe(REPLACEMENT);
        }

        [TestMethod]
        public void CreateReplacementWorksWithMultipleReplacement()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            const string REPLACEMENT2 = "MyReplacement";
            var dc = new DisassemblyAnnotation();

            // --- Act
            var result1 = dc.SetLiteralReplacement(0x1000, REPLACEMENT);
            var result2 = dc.SetLiteralReplacement(0x2000, REPLACEMENT2);

            // --- Assert
            result1.ShouldBe(true);
            dc.LiteralReplacements.Count.ShouldBe(2);
            dc.LiteralReplacements[0x1000].ShouldBe(REPLACEMENT);
            result2.ShouldBe(true);
            dc.LiteralReplacements[0x2000].ShouldBe(REPLACEMENT2);
        }

        [TestMethod]
        public void CreateReplacementOverwritesExistingReplacement()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            const string REPLACEMENT2 = "MyReplacement2";
            var dc = new DisassemblyAnnotation();
            dc.SetLiteralReplacement(0x1000, REPLACEMENT);

            // --- Act
            var result = dc.SetLiteralReplacement(0x1000, REPLACEMENT2);

            // --- Assert
            result.ShouldBe(true);
            dc.LiteralReplacements.Count.ShouldBe(1);
            dc.LiteralReplacements[0x1000].ShouldBe(REPLACEMENT2);
        }

        [TestMethod]
        public void CreateReplacementRemovesNullReplacement()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();
            dc.SetLiteralReplacement(0x1000, REPLACEMENT);

            // --- Act
            var result = dc.SetLiteralReplacement(0x1000, null);

            // --- Assert
            result.ShouldBe(true);
            dc.LiteralReplacements.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateReplacementRemovesWhitespaceReplacement()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();
            dc.SetLiteralReplacement(0x1000, REPLACEMENT);

            // --- Act
            var result = dc.SetLiteralReplacement(0x1000, "   ");

            // --- Assert
            result.ShouldBe(true);
            dc.LiteralReplacements.Count.ShouldBe(0);
        }

        [TestMethod]
        public void CreateReplacementHandlesNoRemove()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();
            dc.SetLiteralReplacement(0x1000, REPLACEMENT);

            // --- Act
            var result = dc.SetComment(0x2000, null);

            // --- Assert
            result.ShouldBe(false);
            dc.LiteralReplacements.Count.ShouldBe(1);
        }

        [TestMethod]
        public void ApplyLiteralCanRemoveReplacement()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();
            dc.SetLiteralReplacement(0x1000, REPLACEMENT);

            // --- Act
            var result = dc.ApplyLiteral(0x1000, 0x0000, null);

            // --- Assert
            result.ShouldBeNull();
            dc.LiteralReplacements.Count.ShouldBe(0);
        }

        [TestMethod]
        public void ApplyLiteralCreatesNewSymbol()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();

            // --- Act
            var result = dc.ApplyLiteral(0x1000, 0x2000, REPLACEMENT);

            // --- Assert
            result.ShouldBeNull();
            dc.Literals[0x2000].Count.ShouldBe(1);
            dc.Literals[0x2000][0].ShouldBe(REPLACEMENT);
            dc.LiteralReplacements.Count.ShouldBe(1);
            dc.LiteralReplacements[0x1000].ShouldBe(REPLACEMENT);
        }

        [TestMethod]
        public void ApplyLiteralUsesExistingSymbol()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();
            dc.AddLiteral(0x2000, REPLACEMENT);

            // --- Act
            var result = dc.ApplyLiteral(0x1000, 0x2000, REPLACEMENT);

            // --- Assert
            result.ShouldBeNull();
            dc.Literals[0x2000].Count.ShouldBe(1);
            dc.Literals[0x2000][0].ShouldBe(REPLACEMENT);
            dc.LiteralReplacements.Count.ShouldBe(1);
            dc.LiteralReplacements[0x1000].ShouldBe(REPLACEMENT);
        }

        [TestMethod]
        public void ApplyLiteralDeniesWrongSymbolValue()
        {
            // --- Arrange
            const string REPLACEMENT = "MyReplacement";
            var dc = new DisassemblyAnnotation();
            dc.AddLiteral(0x3000, REPLACEMENT);

            // --- Act
            var result = dc.ApplyLiteral(0x1000, 0x2000, REPLACEMENT);

            // --- Assert
            result.ShouldNotBeNull();
            dc.Literals[0x3000].Count.ShouldBe(1);
            dc.Literals[0x3000][0].ShouldBe(REPLACEMENT);
            dc.LiteralReplacements.Count.ShouldBe(0);
        }

        [TestMethod]
        public void SerializationWorksAsExpected()
        {
            // --- Arrange
            var dc = new DisassemblyAnnotation();
            dc.SetLabel(0x0100, "FirstLabel");
            dc.SetLabel(0x0200, "SecondLabel");
            dc.SetComment(0x0100, "FirstComment");
            dc.SetComment(0x0200, "SecondComment");
            dc.SetPrefixComment(0x0100, "FirstPrefixComment");
            dc.SetPrefixComment(0x0200, "SecondPrefixComment");
            dc.AddLiteral(0x0000, "Entry");
            dc.AddLiteral(0x0000, "Start");
            dc.AddLiteral(0x0028, "Calculator");
            dc.MemoryMap.Add(new MemorySection(0x0000, 0x3BFF));
            dc.MemoryMap.Add(new MemorySection(0x3C00, 0x3FFF, MemorySectionType.ByteArray));
            dc.SetLiteralReplacement(0x100, "Entry");
            dc.SetLiteralReplacement(0x1000, "Calculator");

            // --- Act
            var serialized = dc.Serialize();
            var back = DisassemblyAnnotation.Deserialize(serialized);

            // --- Assert
            dc.Labels.Count.ShouldBe(back.Labels.Count);
            foreach (var item in dc.Labels)
            {
                back.Labels[item.Key].ShouldBe(item.Value);
            }
            dc.Comments.Count.ShouldBe(back.Comments.Count);
            foreach (var item in dc.Comments)
            {
                back.Comments[item.Key].ShouldBe(item.Value);
            }
            dc.PrefixComments.Count.ShouldBe(back.PrefixComments.Count);
            foreach (var item in dc.PrefixComments)
            {
                back.PrefixComments[item.Key].ShouldBe(item.Value);
            }
            dc.Literals.Count.ShouldBe(back.Literals.Count);
            foreach (var item in dc.Literals)
            {
                back.Literals[item.Key].ForEach(v => dc.Literals[item.Key].ShouldContain(v));
                dc.Literals[item.Key].ForEach(v => back.Literals[item.Key].ShouldContain(v));
            }
            dc.LiteralReplacements.Count.ShouldBe(back.LiteralReplacements.Count);
            foreach (var item in dc.LiteralReplacements)
            {
                back.LiteralReplacements[item.Key].ShouldBe(item.Value);
            }
            dc.MemoryMap.Count.ShouldBe(back.MemoryMap.Count);
            for (var i = 0; i < dc.MemoryMap.Count; i++)
            {
                dc.MemoryMap[i].ShouldBe(back.MemoryMap[i]);
            }
        }

        [TestMethod]
        public void MergeWorksAsExpected()
        {
            // --- Arrange
            var dc = new DisassemblyAnnotation();
            dc.SetLabel(0x0100, "FirstLabel");
            dc.SetLabel(0x0200, "SecondLabel");
            dc.SetComment(0x0100, "FirstComment");
            dc.SetComment(0x0200, "SecondComment");
            dc.SetPrefixComment(0x0100, "FirstPrefixComment");
            dc.SetPrefixComment(0x0200, "SecondPrefixComment");
            dc.AddLiteral(0x0000, "Entry");
            dc.AddLiteral(0x0000, "Start");
            dc.AddLiteral(0x0028, "Calculator");
            dc.MemoryMap.Add(new MemorySection(0x0000, 0x3BFF));
            dc.MemoryMap.Add(new MemorySection(0x3C00, 0x3FFF, MemorySectionType.ByteArray));
            dc.SetLiteralReplacement(0x100, "Entry");
            dc.SetLiteralReplacement(0x1000, "Calculator");

            // --- Act
            var odc = new DisassemblyAnnotation();
            odc.SetLabel(0x0200, "SecondLabelA");
            odc.SetLabel(0x0300, "ThirdLabel");
            odc.SetComment(0x0100, "FirstCommentA");
            odc.SetComment(0x0300, "ThirdComment");
            odc.SetPrefixComment(0x0200, "SecondPrefixCommentA");
            odc.SetPrefixComment(0x0300, "ThirdPrefixComment");
            odc.AddLiteral(0x0000, "Start");
            odc.AddLiteral(0x0028, "CalculatorA");
            odc.MemoryMap.Add(new MemorySection(0x3C00, 0x5BFF, MemorySectionType.ByteArray));
            odc.SetLiteralReplacement(0x100, "Entry");
            odc.SetLiteralReplacement(0x200, "Other");
            odc.SetLiteralReplacement(0x1000, "CalculatorA");
            dc.Merge(odc);

            // --- Assert
            dc.Labels.Count.ShouldBe(3);
            dc.Labels[0x100].ShouldBe("FirstLabel");
            dc.Labels[0x200].ShouldBe("SecondLabelA");
            dc.Labels[0x300].ShouldBe("ThirdLabel");
            dc.Comments.Count.ShouldBe(3);
            dc.Comments[0x100].ShouldBe("FirstCommentA");
            dc.Comments[0x200].ShouldBe("SecondComment");
            dc.Comments[0x300].ShouldBe("ThirdComment");
            dc.PrefixComments.Count.ShouldBe(3);
            dc.PrefixComments[0x100].ShouldBe("FirstPrefixComment");
            dc.PrefixComments[0x200].ShouldBe("SecondPrefixCommentA");
            dc.PrefixComments[0x300].ShouldBe("ThirdPrefixComment");
            dc.Literals.Count.ShouldBe(2);
            dc.Literals[0x0000].Count.ShouldBe(2);
            dc.Literals[0x0000].ShouldContain("Start");
            dc.Literals[0x0000].ShouldContain("Entry");
            dc.Literals[0x0028].Count.ShouldBe(2);
            dc.Literals[0x0028].ShouldContain("Calculator");
            dc.Literals[0x0028].ShouldContain("CalculatorA");
            dc.MemoryMap.Count.ShouldBe(2);
            dc.MemoryMap[0].ShouldBe(new MemorySection(0x0000, 0x3BFF));
            dc.MemoryMap[1].ShouldBe(new MemorySection(0x3C00, 0x5BFF, MemorySectionType.ByteArray));
            dc.LiteralReplacements.Count.ShouldBe(3);
            dc.LiteralReplacements[0x100].ShouldBe("Entry");
            dc.LiteralReplacements[0x200].ShouldBe("Other");
            dc.LiteralReplacements[0x1000].ShouldBe("CalculatorA");
        }
    }
}
