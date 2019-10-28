using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class LiteralEvaluationTests : ExpressionTestBed
    {
        [TestMethod]
        [DataRow("0", 0)]
        [DataRow("12345", 12345)]
        [DataRow("99999", 34463)]
        public void DecimalLiteralEvaluationWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("#0", 0)]
        [DataRow("0H", 0)]
        [DataRow("#12AC", 0x12AC)]
        [DataRow("$12AC", 0x12AC)]
        [DataRow("0F78AH", 0xF78A)]
        [DataRow("78AFH", 0x78AF)]
        [DataRow("0F78Ah", 0xF78A)]
        [DataRow("78AFh", 0x78AF)]
        public void HexaDecimalLiteralEvaluationWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("%0", 0)]
        [DataRow("%1", 1)]
        [DataRow("%10101010", 0xAA)]
        [DataRow("%1010101001010101", 0xAA55)]
        [DataRow("0b0", 0)]
        [DataRow("0b1", 1)]
        [DataRow("0b10101010", 0xAA)]
        [DataRow("0b1010101001010101", 0xAA55)]
        public void BinaryLiteralEvaluationWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("%1010_1010", 0xAA)]
        [DataRow("%1010_1010_01_01_01_01", 0xAA55)]
        [DataRow("0b1010_1010", 0xAA)]
        [DataRow("0b_1010_1010", 0xAA)]
        [DataRow("0b1010_1010_01_01_01_01", 0xAA55)]
        public void BinaryLiteralEvaluationWorksWithGroupSeparator(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\'0'", '0')]
        [DataRow("\'A'", 'A')]
        [DataRow("\'\\\"\'", '"')]
        [DataRow("\''\'", '\'')]
        public void CharLiteralEvaluationWorksAsExpected(string source, char expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("$")]
        [DataRow(".")]
        public void CurrentAddressEvaluatesTo08000WhenNoCodeEmitted(string source)
        {
            EvalExpression(source, 0x8000);
        }

        [TestMethod]
        public void CurrentAddressEvaluatesProperly()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("nop \n nop \n nop");

            // --- Act
            var exprNode = ParseExpr("$");
            var result = assembler.Eval(null, exprNode);

            // --- Assert
            result.Value.ShouldBe((ushort)0x8002);
        }

        [TestMethod]
        public void CurrentAddressEvaluatesProperlyWithStartAddress()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("nop \n nop \n nop");
            assembler.GetCurrentAssemblyAddress();
            assembler.CurrentSegment.StartAddress = 0x6800;
            assembler.EmitByte(0x00);
            assembler.EmitByte(0x00);

            // --- Act
            var exprNode = ParseExpr("$");
            var result = assembler.Eval(null, exprNode);

            // --- Assert
            result.Value.ShouldBe((ushort)0x6802);
        }

        [TestMethod]
        public void CurrentAddressEvaluatesProperlyWithDisplacement()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("nop \n nop \n nop");
            assembler.GetCurrentAssemblyAddress();
            assembler.CurrentSegment.StartAddress = 0x6800;
            assembler.CurrentSegment.Displacement = 0x200;
            assembler.EmitByte(0x00);
            assembler.EmitByte(0x00);

            // --- Act
            var exprNode = ParseExpr("$");
            var result = assembler.Eval(null, exprNode);

            // --- Assert
            result.Value.ShouldBe((ushort)0x6A02);
        }

        [TestMethod]
        [DataRow("0.0", 0.0)]
        [DataRow("3.14", 3.14)]
        [DataRow(".25", 0.25)]
        [DataRow("3.14E2", 3.14E2)]
        [DataRow("3.14E+2", 3.14E+2)]
        [DataRow("3.14E-2", 3.14E-2)]
        [DataRow("1E8", 1E8)]
        [DataRow("2E+8", 2E8)]
        [DataRow("3E-8", 3E-8)]
        [DataRow("3E-188888", 0.0)]
        public void RealLiteralEvaluationWorksAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }


    }
}
