using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class LiteralParserTests : ParserTestBed
    {
        [TestMethod]
        public void ZeroLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("0");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)0);
        }

        [TestMethod]
        public void OneLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("1");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)1);
        }


        [TestMethod]
        public void DecimalLiteralParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)12345);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("#23CF");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("23CFH");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("23CFh");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks4()
        {
            // --- Act
            var expr = ParseExpr("0x23CF");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void HexaDecimalLiteralParsingWorks5()
        {
            // --- Act
            var expr = ParseExpr("$23CF");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((ushort)9167);
        }

        [TestMethod]
        public void LabelLikeHexaDecimalLiteralFails()
        {
            // --- Act
            var output = Parse("ff0ah");

            // --- Assert
            output.Compilation.Lines[0].Label.ShouldBe("FF0AH");
        }

        [TestMethod]
        public void CharLiteralParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("\'a'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe('a');
        }

        [TestMethod]
        public void CharLiteralParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("\'\\i'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((byte)0x10);
        }

        [TestMethod]
        public void CharLiteralParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("\'\\xA4\'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((byte)0xA4);
        }

        [TestMethod]
        public void CharLiteralParsingWorks4()
        {
            // --- Act
            var expr = ParseExpr("\'|\'");

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Integer);
            literal.AsWord.ShouldBe((byte)'|');
        }

        [TestMethod]
        [DataRow("true", true)]
        [DataRow(".true", true)]
        [DataRow("TRUE", true)]
        [DataRow(".TRUE", true)]
        [DataRow("false", false)]
        [DataRow(".false", false)]
        [DataRow("FALSE", false)]
        [DataRow(".FALSE", false)]
        public void BoolLiteralParsingWorks(string source, bool expected)
        {
            // --- Act
            var expr = ParseExpr(source);

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Bool);
            literal.AsBool.ShouldBe(expected);
        }

        [TestMethod]
        [DataRow("3.14", 3.14)]
        [DataRow(".25", 0.25)]
        [DataRow("3.14E2", 3.14E2)]
        [DataRow("3.14E+2", 3.14E+2)]
        [DataRow("3.14E-2", 3.14E-2)]
        [DataRow("1E8", 1E8)]
        [DataRow("2E+8", 2E8)]
        [DataRow("3E-8", 3E-8)]
        [DataRow("3E-188888", 0.0)]
        public void RealLiteralParsingWorks(string source, double? expected)
        {
            // --- Act
            var expr = ParseExpr(source);

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.Real);
            if (expected != null)
            {
                literal.AsReal.ShouldBe(expected.Value);
            }
            else
            {
                literal.LiteralValue.ShouldBe(ExpressionValue.Error);                
            }
        }

        [TestMethod]
        [DataRow("\"abc\"", "abc")]
        [DataRow("\"\"", "")]
        public void StringLiteralParsingWorks(string source, string expected)
        {
            // --- Act
            var expr = ParseExpr(source);

            // --- Assert
            var literal = expr as LiteralNode;
            literal.ShouldNotBeNull();
            literal.LiteralValue.Type.ShouldBe(ExpressionValueType.String);
            literal.AsString.ShouldBe(expected);
        }


    }
}
