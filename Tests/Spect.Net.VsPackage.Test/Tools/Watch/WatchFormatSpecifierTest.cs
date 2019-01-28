using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.EvalParser;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.VsPackage.ToolWindows.Watch;
// ReSharper disable StringLiteralTypo

namespace Spect.Net.VsPackage.Test.Tools.Watch
{
    [TestClass]
    public class WatchFormatSpecifierTest
    {
        [TestMethod]
        [DataRow("0", "#00 (0)")]
        [DataRow("127", "#7F (127)")]
        [DataRow("255", "#FF (255)")]
        [DataRow("256", "#0100 (256)")]
        [DataRow("65535", "#FFFF (65535)")]
        [DataRow("#100 * #100", "#00010000 (65536)")]
        [DataRow("2 == 3", "FALSE")]
        [DataRow("2 != 3", "TRUE")]
        public void DefaultFormatSpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :b", "#00 (0)")]
        [DataRow("0 :B", "#00 (0)")]
        [DataRow("127 :b", "#7F (127)")]
        [DataRow("255 :B", "#FF (255)")]
        [DataRow("256 :B", "#00 (0)")]
        [DataRow("65535 :b", "#FF (255)")]
        [DataRow("#100 * #100 :b", "#00 (0)")]
        [DataRow("2 == 3 :B", "#00 (0)")]
        [DataRow("2 != 3 :B", "#01 (1)")]
        [DataRow("0 :-b", "#00 (0)")]
        [DataRow("0 :-B", "#00 (0)")]
        [DataRow("127 :-b", "#7F (127)")]
        [DataRow("255 :-B", "#FF (-1)")]
        [DataRow("254 :-B", "#FE (-2)")]
        [DataRow("256 :-B", "#00 (0)")]
        public void ByteSpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :c", "'\\0x00'")]
        [DataRow("9 :c", "'\\0x09'")]
        [DataRow("32 :C", "' '")]
        [DataRow("65 :C", "'A'")]
        [DataRow("110 :C", "'n'")]
        [DataRow("126 :C", "'~'")]
        [DataRow("127 :C", "'\\0x7F'")]
        [DataRow("256 :c", "'\\0x00'")]
        [DataRow("#0141 :C", "'A'")]
        [DataRow("#100 * #100 + #41 :C", "'A'")]
        [DataRow("2 == 3 :c", "'\\0x00'")]
        [DataRow("2 != 3 :c", "'\\0x01'")]
        public void CharSpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :h4", "#00 #00")]
        [DataRow("0 :H4", "#00 #00")]
        [DataRow("127 :h4", "#7F #00")]
        [DataRow("255 :h4", "#FF #00")]
        [DataRow("256 :h4", "#00 #01")]
        [DataRow("65535 :h4", "#FF #FF")]
        [DataRow("#100 * #100 :H4", "#00 #00")]
        [DataRow("2 == 3 :H4", "#00 #00")]
        [DataRow("2 != 3 :H4", "#01 #00")]
        public void H4SpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :h8", "#00 #00 #00 #00")]
        [DataRow("0 :H8", "#00 #00 #00 #00")]
        [DataRow("127 :h8", "#7F #00 #00 #00")]
        [DataRow("255 :h8", "#FF #00 #00 #00")]
        [DataRow("256 :h8", "#00 #01 #00 #00")]
        [DataRow("65535 :h8", "#FF #FF #00 #00")]
        [DataRow("#100 * #100 :H8", "#00 #00 #01 #00")]
        [DataRow("2 == 3 :H8", "#00 #00 #00 #00")]
        [DataRow("2 != 3 :H8", "#01 #00 #00 #00")]
        [DataRow("#CD12 * #100 + #A5 :H8", "#A5 #12 #CD #00")]
        public void H8SpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :w", "#0000 (0)")]
        [DataRow("0 :W", "#0000 (0)")]
        [DataRow("127 :W", "#007F (127)")]
        [DataRow("255 :w", "#00FF (255)")]
        [DataRow("256 :w", "#0100 (256)")]
        [DataRow("65535 :w", "#FFFF (65535)")]
        [DataRow("#100 * #100 :W", "#0000 (0)")]
        [DataRow("2 == 3 :w", "#0000 (0)")]
        [DataRow("2 != 3 :W", "#0001 (1)")]
        [DataRow("#CD12 * #100 + #A5 :W", "#12A5 (4773)")]
        [DataRow("65535 :-w", "#FFFF (-1)")]
        [DataRow("65534 :-w", "#FFFE (-2)")]
        public void WordSpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :dw", "#00000000 (0)")]
        [DataRow("0 :DW", "#00000000 (0)")]
        [DataRow("127 :DW", "#0000007F (127)")]
        [DataRow("255 :dw", "#000000FF (255)")]
        [DataRow("256 :dw", "#00000100 (256)")]
        [DataRow("65535 :dw", "#0000FFFF (65535)")]
        [DataRow("#100 * #100 :DW", "#00010000 (65536)")]
        [DataRow("2 == 3 :dw", "#00000000 (0)")]
        [DataRow("2 != 3 :DW", "#00000001 (1)")]
        [DataRow("#CD12 * #100 + #A5 :DW", "#00CD12A5 (13439653)")]
        [DataRow("#ffff * #1000 * #10 + #FFFF :-dw", "#FFFFFFFF (-1)")]
        [DataRow("#ffff * #1000 * #10 + #FFFE :-dw", "#FFFFFFFE (-2)")]
        public void DWordSpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :%8", "00000000")]
        [DataRow("0 :%8", "00000000")]
        [DataRow("127 :%8", "01111111")]
        [DataRow("255 :%8", "11111111")]
        [DataRow("256 :%8", "00000000")]
        [DataRow("65535 :%8", "11111111")]
        [DataRow("#100 * #100 :%8", "00000000")]
        [DataRow("2 == 3 :%8", "00000000")]
        [DataRow("2 != 3 :%8", "00000001")]
        public void Bit8SpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :%16", "00000000 00000000")]
        [DataRow("127 :%16", "00000000 01111111")]
        [DataRow("255 :%16", "00000000 11111111")]
        [DataRow("256 :%16", "00000001 00000000")]
        [DataRow("65535 :%16", "11111111 11111111")]
        [DataRow("#100 * #100 :%16", "00000000 00000000")]
        [DataRow("2 == 3 :%16", "00000000 00000000")]
        [DataRow("2 != 3 :%16", "00000000 00000001")]
        [DataRow("#CD12 * #100 + #A5 :%16", "00010010 10100101")]
        public void Bit16SpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        [TestMethod]
        [DataRow("0 :%32", "00000000 00000000 00000000 00000000")]
        [DataRow("127 :%32", "00000000 00000000 00000000 01111111")]
        [DataRow("255 :%32", "00000000 00000000 00000000 11111111")]
        [DataRow("256 :%32", "00000000 00000000 00000001 00000000")]
        [DataRow("65535 :%32", "00000000 00000000 11111111 11111111")]
        [DataRow("#100 * #100 :%32", "00000000 00000001 00000000 00000000")]
        [DataRow("2 == 3 :%32", "00000000 00000000 00000000 00000000")]
        [DataRow("2 != 3 :%32", "00000000 00000000 00000000 00000001")]
        [DataRow("#CD12 * #100 + #A5 :%32", "00000000 11001101 00010010 10100101")]
        public void Bit32SpecifierWorksAsExpected(string expression, string expected)
        {
            TestFormat(expression, expected);
        }

        private void TestFormat(string expression, string result)
        {
            var inputStream = new AntlrInputStream(expression);
            var lexer = new Z80EvalLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var evalParser = new Z80EvalParser(tokenStream);
            var context = evalParser.compileUnit();
            var visitor = new Z80EvalVisitor();
            var z80Expr = (Z80ExpressionNode)visitor.Visit(context);
            evalParser.SyntaxErrors.Count.ShouldBe(0);
            var value = z80Expr.Expression.Evaluate(new FakeEvaluationContext());
            var formatted =
                WatchToolWindowViewModel.FormatWatchExpression(z80Expr.Expression, value,
                    z80Expr.FormatSpecifier?.Format);
            formatted.ShouldBe(result);
        }

        class FakeEvaluationContext : IExpressionEvaluationContext
        {
            public ExpressionValue GetSymbolValue(string symbol)
            {
                return ExpressionValue.Error;
            }

            public ExpressionValue GetZ80RegisterValue(string registerName, out bool is8Bit)
            {
                is8Bit = false;
                return ExpressionValue.Error;
            }

            public ExpressionValue GetZ80FlagValue(string flagName)
            {
                return ExpressionValue.Error;
            }

            public ExpressionValue GetMemoryIndirectValue(ExpressionValue address)
            {
                return ExpressionValue.Error;
            }
        }
    }
}
