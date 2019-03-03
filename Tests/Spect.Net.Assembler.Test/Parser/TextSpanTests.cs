using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class TextSpanTests : ParserTestBed
    {
        [TestMethod]
        public void LabelSpanWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("   StartLabel: ld a,b");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.LabelSpan.Start.ShouldBe(3);
            line.LabelSpan.End.ShouldBe(13);
        }

        [TestMethod]
        public void KeywordSpanWorksWithPragma()
        {
            // --- Act
            var visitor = Parse("   .org #8000");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.LabelSpan.ShouldBeNull();
            line.KeywordSpan.Start.ShouldBe(3);
            line.KeywordSpan.End.ShouldBe(7);
        }

        [TestMethod]
        public void KeywordSpanWorksWithDirective()
        {
            // --- Act
            var visitor = Parse(" #ifdef MY_SYMBOL");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.LabelSpan.ShouldBeNull();
            line.KeywordSpan.Start.ShouldBe(1);
            line.KeywordSpan.End.ShouldBe(7);
        }

        [TestMethod]
        public void KeywordSpanWorksWithOperation()
        {
            // --- Act
            var visitor = Parse(" bit 7,c");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.LabelSpan.ShouldBeNull();
            line.KeywordSpan.Start.ShouldBe(1);
            line.KeywordSpan.End.ShouldBe(4);
        }

        [TestMethod]
        public void LabelAndOperationWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("MyLabel: bit 7,c");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.LabelSpan.Start.ShouldBe(0);   
            line.LabelSpan.End.ShouldBe(7);
            line.KeywordSpan.Start.ShouldBe(9);
            line.KeywordSpan.End.ShouldBe(12);
        }

        [TestMethod]
        public void SingleNumberSpanWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("bit 7,c");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.Numbers.Count.ShouldBe(1);
            line.Numbers[0].Start.ShouldBe(4);
            line.Numbers[0].End.ShouldBe(5);
        }

        [TestMethod]
        public void MultipleNumberSpansWorkAsExpected1()
        {
            // --- Act
            var visitor = Parse("ld hl,#1234+5678-3");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.Numbers.Count.ShouldBe(3);
            line.Numbers[0].Start.ShouldBe(6);
            line.Numbers[0].End.ShouldBe(11);
            line.Numbers[1].Start.ShouldBe(12);
            line.Numbers[1].End.ShouldBe(16);
            line.Numbers[2].Start.ShouldBe(17);
            line.Numbers[2].End.ShouldBe(18);
        }

        [TestMethod]
        public void SingleIdentifierSpanWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("ld a,myvalue");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.Identifiers.Count.ShouldBe(1);
            line.Identifiers[0].Start.ShouldBe(5);
            line.Identifiers[0].End.ShouldBe(12);
        }

        [TestMethod]
        public void MultipleIdentifierSpansWorkAsExpected()
        {
            // --- Act
            var visitor = Parse("ld hl,value1+othervalue");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.Identifiers.Count.ShouldBe(2);
            line.Identifiers[0].Start.ShouldBe(6);
            line.Identifiers[0].End.ShouldBe(12);
            line.Identifiers[1].Start.ShouldBe(13);
            line.Identifiers[1].End.ShouldBe(23);
        }

        [TestMethod]
        public void FullLineCommentSpanWorksAsExpected()
        {
            // --- Act
            var visitor = Parse("  ; This is a comment\r\n");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.Comment.ShouldBe("; This is a comment");
            line.CommentSpan.Start.ShouldBe(2);
            line.CommentSpan.End.ShouldBe(21);
        }

        [TestMethod]
        public void FaultyLineSetsUpSpans()
        {
            // --- Act
            var visitor = ParseAsmLine("mylabel:  ld mysymbol+#2345,");

            // --- Assert

        }

        [TestMethod]
        public void InstructionSpanWorksWithLabelOnly()
        {
            // --- Act
            var visitor = Parse("mySymbol:");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.InstructionSpan.Start.ShouldBe(0);
            line.InstructionSpan.End.ShouldBe(0);
        }

        [TestMethod]
        public void InstructionSpanWorksWithLabelAndInstruction()
        {
            // --- Act
            var visitor = Parse("mySymbol: ld hl,#1234");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.InstructionSpan.ShouldNotBeNull();
            line.InstructionSpan.Start.ShouldBe(10);
            line.InstructionSpan.End.ShouldBe(21);
        }

        [TestMethod]
        public void InstructionSpanWorksWithLabelAndComment()
        {
            // --- Act
            var visitor = Parse("mySymbol: ld hl,#1234 ; This is a comment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.InstructionSpan.ShouldNotBeNull();
            line.InstructionSpan.Start.ShouldBe(10);
            line.InstructionSpan.End.ShouldBe(21);
        }

        [TestMethod]
        public void InstructionSpanWorksWithComment()
        {
            // --- Act
            var visitor = Parse("ld hl,#1234 ; This is a comment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.InstructionSpan.ShouldNotBeNull();
            line.InstructionSpan.Start.ShouldBe(0);
            line.InstructionSpan.End.ShouldBe(11);
        }

        [TestMethod]
        public void InstructionSpanWorksWithCommentOnly()
        {
            // --- Act
            var visitor = Parse("  ; This is a comment");

            // --- Assert
            visitor.Compilation.Lines.Count.ShouldBe(1);
            var line = visitor.Compilation.Lines[0];
            line.ShouldNotBeNull();
            line.InstructionSpan.Start.ShouldBe(2);
            line.InstructionSpan.End.ShouldBe(2);
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private Z80AsmVisitor ParseAsmLine(string textToParse)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80AsmLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80AsmParser(tokenStream);
            var context = parser.asmline();
            var visitor = new Z80AsmVisitor();
            visitor.Visit(context);
            return visitor;
        }
    }
}