using AntlrZ80Asm.SyntaxTree.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace AntlrZ80Asm.Test.Parser
{
    [TestClass]
    public class ExpressionParserTests : ParserTestBed
    {
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
        public void HexaDecimalLiteralParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("#23CF");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("23CFH");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("23CFh");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void OrOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 | 23456");

            // --- Assert
            var literal = expr as BitwiseOrOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.LiteralValue.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.LiteralValue.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedOrOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 | 1111| 23456");

            // --- Assert
            var binaryOp = expr as BitwiseOrOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as BitwiseOrOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.LiteralValue.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.LiteralValue.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.LiteralValue.ShouldBe((ushort)23456);
        }

    }
}
