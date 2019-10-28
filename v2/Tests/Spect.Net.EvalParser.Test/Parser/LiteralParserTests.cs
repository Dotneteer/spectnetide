using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.EvalParser.SyntaxTree;

namespace Spect.Net.EvalParser.Test.Parser
{
    [TestClass]
    public class LiteralParserTests: ParserTestBed
    {
        [TestMethod]
        public void ZeroLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("0");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0);
        }

        [TestMethod]
        public void OneLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("1");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)1);
        }


        [TestMethod]
        public void DecimalLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)12345);
        }

        [TestMethod]
        public void HexadecimalLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("#23CF");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexadecimalLiteralParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("23CFH");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexadecimalLiteralParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("23CFh");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexadecimalLiteralParsingWorks4()
        {
            // --- Act
            var expr = ParseExpr("0x23CF");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void CharLiteralParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("'a'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe('a');
        }

        [TestMethod]
        public void CharLiteralParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("'\\i'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((byte)0x10);
        }

        [TestMethod]
        public void CharLiteralParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("'\\xA4'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((byte)0xA4);
        }

        [TestMethod]
        public void CharLiteralParsingWorks4()
        {
            // --- Act
            var expr = ParseExpr("'|'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((byte)'|');
        }

        [TestMethod]
        public void BinaryLiteralParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("%1101");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0x0D);
        }

        [TestMethod]
        public void BinaryLiteralParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("0b1101");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0x0D);
        }

        [TestMethod]
        public void BinaryLiteralParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("0b11010101");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0x0D5);
        }

        [TestMethod]
        public void BinaryLiteralParsingWorks4()
        {
            // --- Act
            var expr = ParseExpr("0b_11010101");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0x0D5);
        }

        [TestMethod]
        public void BinaryLiteralParsingWorks5()
        {
            // --- Act
            var expr = ParseExpr("%_1101_0101");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0x0D5);
        }

        [TestMethod]
        public void BinaryLiteralParsingWorks6()
        {
            // --- Act
            var expr = ParseExpr("%1101_0101_1100_0110");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((uint)0x0D5C6);
        }

    }
}
