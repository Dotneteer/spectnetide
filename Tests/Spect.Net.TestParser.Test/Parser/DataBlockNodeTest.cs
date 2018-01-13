using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class DataBlockNodeTest
    {
        [TestMethod]
        public void EmptyDataBlockWorks()
        {
            // --- Act
            var visitor = ParseDataBlock("data { }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(7);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);
            visitor.DataMembers.Count.ShouldBe(0);
        }

        [TestMethod]
        public void DataBlockWithSingleValueWorks()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myValue: 123; }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(21);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as ValueMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myValue");
            member1.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DataBlockWithMultipleValueWorks()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myValue: 123; myValue2: #fe; }");

            // --- Assert
            visitor.DataMembers.Count.ShouldBe(2);
            var member1 = visitor.DataMembers[0] as ValueMemberNode;
            member1.ShouldNotBeNull();
            member1.Id.ShouldBe("myValue");
            member1.Expr.ShouldBeOfType<LiteralNode>();

            var member2 = visitor.DataMembers[1] as ValueMemberNode;
            member2.ShouldNotBeNull();
            member2.Id.ShouldBe("myValue2");
            member2.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DataBlockWithSingleBytePatternWorks1()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { byte #01; }; }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(30);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myArray");
            member1.Patterns.Count.ShouldBe(1);
            var pattern1 = member1.Patterns[0] as BytePatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.Bytes.Count.ShouldBe(1);
            pattern1.Bytes[0].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DataBlockWithSingleBytePatternWorks2()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { byte #01, #02; } }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(34);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myArray");
            member1.Patterns.Count.ShouldBe(1);
            var pattern1 = member1.Patterns[0] as BytePatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.Bytes.Count.ShouldBe(2);
            pattern1.Bytes[0].ShouldBeOfType<LiteralNode>();
            pattern1.Bytes[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DataBlockWithSingleWordPatternWorks1()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { word #01A0; }; }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(32);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myArray");
            member1.Patterns.Count.ShouldBe(1);
            var pattern1 = member1.Patterns[0] as WordPatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.Words.Count.ShouldBe(1);
            pattern1.Words[0].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DataBlockWithSingleWordPatternWorks2()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { word #01A0, #02BC; } }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(38);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myArray");
            member1.Patterns.Count.ShouldBe(1);
            var pattern1 = member1.Patterns[0] as WordPatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.Words.Count.ShouldBe(2);
            pattern1.Words[0].ShouldBeOfType<LiteralNode>();
            pattern1.Words[1].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void DataBlockWithSingleTextPatternWorks1()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { text \"aaa\"; }; }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(32);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myArray");
            member1.Patterns.Count.ShouldBe(1);
            var pattern1 = member1.Patterns[0] as TextPatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.String.ShouldBe("aaa");
        }

        [TestMethod]
        public void DataBlockWithSingleTextPatternWorks2()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { text \"aaa\"; text \"bbb\"; }; }");

            // --- Assert
            visitor.Span.StartLine.ShouldBe(1);
            visitor.Span.StartColumn.ShouldBe(0);
            visitor.Span.EndLine.ShouldBe(1);
            visitor.Span.EndColumn.ShouldBe(44);

            var kw = visitor.DataKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(0);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(3);

            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            var mkw = member1.IdSpan;
            mkw.StartLine.ShouldBe(1);
            mkw.StartColumn.ShouldBe(7);
            mkw.EndLine.ShouldBe(1);
            mkw.EndColumn.ShouldBe(13);
            member1.Id.ShouldBe("myArray");
            member1.Patterns.Count.ShouldBe(2);
            var pattern1 = member1.Patterns[0] as TextPatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.String.ShouldBe("aaa");
            var pattern2 = member1.Patterns[1] as TextPatternNode;
            pattern2.ShouldNotBeNull();
            pattern2.String.ShouldBe("bbb");
        }

        [TestMethod]
        public void DataBlockWithSingleTextPatternsWorks()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myArray { byte #01; word #0234, 1234; text \"aaa\"; } }");

            // --- Assert
            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as MemoryPatternMemberNode;
            member1.ShouldNotBeNull();
            member1.Patterns.Count.ShouldBe(3);
            var pattern1 = member1.Patterns[0] as BytePatternNode;
            pattern1.ShouldNotBeNull();
            pattern1.Bytes[0].ShouldBeOfType<LiteralNode>();
            var pattern2 = member1.Patterns[1] as WordPatternNode;
            pattern2.ShouldNotBeNull();
            pattern2.Words.Count.ShouldBe(2);
            pattern2.Words[0].ShouldBeOfType<LiteralNode>();
            pattern2.Words[1].ShouldBeOfType<LiteralNode>();
            var pattern3 = member1.Patterns[2] as TextPatternNode;
            pattern3.ShouldNotBeNull();
            pattern3.String.ShouldBe("aaa");
        }

        [TestMethod]
        public void DataBlockWithSinglePortMockWorks1()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myMock <#fe>: {100: 200}; }");

            // --- Assert
            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as PortMockMemberNode;
            member1.ShouldNotBeNull();
            member1.Pulses.Count.ShouldBe(1);
            member1.Expr.ShouldBeOfType<LiteralNode>();
            var pulse1 = member1.Pulses[0];
            pulse1.ValueExpr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse1Expr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse2Expr.ShouldBeNull();
            pulse1.IsInterval.ShouldBeFalse();
        }

        [TestMethod]
        public void DataBlockWithSinglePortMockWorks2()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myMock <#fe>: {100: 200..300}; }");

            // --- Assert
            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as PortMockMemberNode;
            member1.ShouldNotBeNull();
            member1.Pulses.Count.ShouldBe(1);
            member1.Expr.ShouldBeOfType<LiteralNode>();
            var pulse1 = member1.Pulses[0];
            pulse1.ValueExpr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse1Expr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse2Expr.ShouldBeOfType<LiteralNode>();
            pulse1.IsInterval.ShouldBeTrue();
        }

        [TestMethod]
        public void DataBlockWithSinglePortMockWorks3()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myMock <#fe>: {100: 200,300}; }");

            // --- Assert
            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as PortMockMemberNode;
            member1.ShouldNotBeNull();
            member1.Pulses.Count.ShouldBe(1);
            member1.Expr.ShouldBeOfType<LiteralNode>();
            var pulse1 = member1.Pulses[0];
            pulse1.ValueExpr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse1Expr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse2Expr.ShouldBeOfType<LiteralNode>();
            pulse1.IsInterval.ShouldBeFalse();
        }

        [TestMethod]
        public void DataBlockWithSinglePortMockWorks4()
        {
            // --- Act
            var visitor = ParseDataBlock("data { myMock <#fe>: {100: 200,300}, {100:200}, {100:200..300}; }");

            // --- Assert
            visitor.DataMembers.Count.ShouldBe(1);
            var member1 = visitor.DataMembers[0] as PortMockMemberNode;
            member1.ShouldNotBeNull();
            member1.Pulses.Count.ShouldBe(3);
            member1.Expr.ShouldBeOfType<LiteralNode>();
            var pulse1 = member1.Pulses[0];
            pulse1.ValueExpr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse1Expr.ShouldBeOfType<LiteralNode>();
            pulse1.Pulse2Expr.ShouldBeOfType<LiteralNode>();
            pulse1.IsInterval.ShouldBeFalse();
            var pulse2 = member1.Pulses[1];
            pulse2.ValueExpr.ShouldBeOfType<LiteralNode>();
            pulse2.Pulse1Expr.ShouldBeOfType<LiteralNode>();
            pulse2.Pulse2Expr.ShouldBeNull();
            pulse2.IsInterval.ShouldBeFalse();
            var pulse3 = member1.Pulses[2];
            pulse3.ValueExpr.ShouldBeOfType<LiteralNode>();
            pulse3.Pulse1Expr.ShouldBeOfType<LiteralNode>();
            pulse3.Pulse2Expr.ShouldBeOfType<LiteralNode>();
            pulse3.IsInterval.ShouldBeTrue();
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static DataBlockNode ParseDataBlock(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.dataBlock();
            var visitor = new Z80TestVisitor();
            var result = (DataBlockNode)visitor.VisitDataBlock(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

    }
}
