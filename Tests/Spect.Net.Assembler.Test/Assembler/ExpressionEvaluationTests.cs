using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ExpressionEvaluationTests : AssemblerTestBed
    {
        [TestMethod]
        public void DecimalLiteralEvaluationWorksAsExpected()
        {
            EvalExpression("0", 0);
            EvalExpression("12345", 12345);
            EvalExpression("99999", 34463); // Trim 16 bits
        }

        [TestMethod]
        public void HexaDecimalLiteralEvaluationWorksAsExpected()
        {
            EvalExpression("#0", 0);
            EvalExpression("0H", 0);
            EvalExpression("#12AC", 0x12AC);
            EvalExpression("F78AH", 0xF78A);
        }

        [TestMethod]
        public void CharLiteralEvaluationWorksAsExpected()
        {
            EvalExpression("\"0\"", '0');
            EvalExpression("\"A\"", 'A');
            EvalExpression("\"\"\"", '"');
        }

        [TestMethod]
        public void CurrentAddressEvaluatesTo08000WhenNoCodeEmitted()
        {
            EvalExpression("$", 0x8000);
        }

        [TestMethod]
        public void CurrentAddressEvaluatesProperly()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("");
            assembler.EmitByte(0x00);
            assembler.EmitByte(0x00);

            // --- Act
            var exprNode = ParseExpr("$");
            var result = assembler.Eval(exprNode);

            // --- Assert
            result.ShouldBe((ushort)0x8002);
        }

        [TestMethod]
        public void CurrentAddressEvaluatesProperlyWithStartAddress()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("");
            assembler.GetCurrentAssemblyAddress();
            assembler.CurrentSegment.StartAddress = 0x6800;
            assembler.EmitByte(0x00);
            assembler.EmitByte(0x00);

            // --- Act
            var exprNode = ParseExpr("$");
            var result = assembler.Eval(exprNode);

            // --- Assert
            result.ShouldBe((ushort)0x6802);
        }

        [TestMethod]
        public void CurrentAddressEvaluatesProperlyWithDisplacement()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("");
            assembler.GetCurrentAssemblyAddress();
            assembler.CurrentSegment.StartAddress = 0x6800;
            assembler.CurrentSegment.Displacement = 0x200;
            assembler.EmitByte(0x00);
            assembler.EmitByte(0x00);

            // --- Act
            var exprNode = ParseExpr("$");
            var result = assembler.Eval(exprNode);

            // --- Assert
            result.ShouldBe((ushort)0x6A02);
        }

        [TestMethod]
        public void UnknowSymbolEvaluatesToNull()
        {
            EvalExpression("UNKNOWN", null);
        }

        [TestMethod]
        public void KnowSymbolEvaluatesToItsValue()
        {
            var symbols = new Dictionary<string, ushort>
            {
                { "known", 0x23EA },
                { "other", 0xDD34 },
            };
            EvalExpression("UNKNOWN", null, symbols: symbols);
            EvalExpression("KNOWN", 0x23EA, symbols: symbols);
            EvalExpression("known", 0x23EA, symbols: symbols);
        }

        [TestMethod]
        public void UnaryPlusWorksAsExpected()
        {
            EvalExpression("+0", 0);
            EvalExpression("+12345", 12345);
            EvalExpression("+99999", 34463); // Trim 16 bits
            EvalExpression("+#12AC", 0x12AC);
        }

        [TestMethod]
        public void UnaryMinusWorksAsExpected()
        {
            EvalExpression("-0", 0);
            EvalExpression("-12345", 53191); // 16 bit ushort!
            EvalExpression("-99999", 31073); // Trim 16 bits
            EvalExpression("-#12AC", 0xED54);
        }

        [TestMethod]
        public void MultiplicativeOpsWorkAsExpected()
        {
            EvalExpression("0 * 3", 0);
            EvalExpression("35000 * 43000", 31296);
            EvalExpression("12 * 23", 276);
            EvalExpression("80 / 12", 6);
            EvalExpression("99999 / 123", 280);
            EvalExpression("408 % 5", 3);
        }

        [TestMethod]
        public void DivideByZeroRaisesError()
        {
            EvalExpression("112 / 0", null, true);
            EvalExpression("112 / [3+4-7]", null, true);
        }

        [TestMethod]
        public void CompilerLogsErrorWHenDivideByZero()
        {
            // --- Arrange
            var assembler = new Z80Assembler();

            // --- Act
            var output = assembler.Compile("ld a,112/0");

            // --- Assert
            output.ErrorCount.ShouldBe(1);
        }

        [TestMethod]
        public void AdditiveOpsWorkAsExpected()
        {
            EvalExpression("0 + 3", 3);
            EvalExpression("12 + 23 - 34", 1);
            EvalExpression("99999 - 99997", 2);
            EvalExpression("8+-3", 5);
            EvalExpression("8-+3", 5);
        }

        [TestMethod]
        public void EqualityOpsWorkAsExpected()
        {
            EvalExpression("0 == 0", 1);
            EvalExpression("0 == 1", 0);
            EvalExpression("23 == 20+3", 1);
            EvalExpression("23-3 == 20+3", 0);
            EvalExpression("0 != 0", 0);
            EvalExpression("0 != 1", 1);
            EvalExpression("23 != 20+3", 0);
            EvalExpression("23-3 != 20+3", 1);
        }

        [TestMethod]
        public void RelationalOpsWorkAsExpected()
        {
            EvalExpression("0 < 1", 1);
            EvalExpression("0 <= 1", 1);
            EvalExpression("23 <= 20+3", 1);
            EvalExpression("23-3 > 20+3", 0);
            EvalExpression("23 >= 20+3", 1);
            EvalExpression("0 >= 1", 0);
            EvalExpression("0 > 1", 0);
            EvalExpression("23 > 20+3", 0);
            EvalExpression("23-3 <= 20+3", 1);
            EvalExpression("23 < 20+3", 0);
        }

        [TestMethod]
        public void LeftShiftOpsWorkAsExpected()
        {
            EvalExpression("1 << 8", 256);
            EvalExpression("3  << 4", 48);
            EvalExpression("#FFFF << 0", 0xFFFF);
            EvalExpression("#FFFF << 12", 0xF000);
            EvalExpression("12345 << 15", 0x8000);
        }

        [TestMethod]
        public void RightShiftOpsWorkAsExpected()
        {
            EvalExpression("256 >> 8", 1);
            EvalExpression("48  >> 4", 3);
            EvalExpression("#FFFF >> 0", 0xFFFF);
            EvalExpression("#FFFF >> 12", 0x000F);
            EvalExpression("32800 >> 15", 0x0001);
            EvalExpression("32800 >> 24", 0x0000);
        }

        [TestMethod]
        public void BitwiseAndOpsWorkAsExpected()
        {
            EvalExpression("#FFFF & #FF", 255);
            EvalExpression("#F8AF & #0880", 0x0880);
        }

        [TestMethod]
        public void BitwiseXorOpsWorkAsExpected()
        {
            EvalExpression("#FFFF ^ #FF", 0xFF00);
            EvalExpression("#F8AF ^ #0880", 0xF02F);
        }

        [TestMethod]
        public void BitwiseOrOpsWorkAsExpected()
        {
            EvalExpression("#FF00 | #00FF", 0xFFFF);
            EvalExpression("#F810 | #FC02", 0xFC12);
        }

        [TestMethod]
        public void ConditionalOpsWorkAsExpected()
        {
            EvalExpression("23+11 > 3 ? 123 : 456", 123);
            EvalExpression("23+11 < 3 ? 123 : 456", 456);
        }

        private void EvalExpression(string expr, ushort? expected, bool hasEvaluationError = false, 
            Dictionary<string, ushort> symbols = null)
        {
            var assembler = new Z80Assembler();
            assembler.Compile("");
            if (symbols != null)
            {
                foreach (var pair in symbols)
                {
                    assembler.SetSymbolValue(pair.Key, pair.Value);
                }
            }
            var exprNode = ParseExpr(expr);
            var result = assembler.Eval(exprNode);
            result.ShouldBe(expected);
            if (hasEvaluationError)
            {
                exprNode.EvaluationError.ShouldNotBeNull();
            }
        }
    }
}
