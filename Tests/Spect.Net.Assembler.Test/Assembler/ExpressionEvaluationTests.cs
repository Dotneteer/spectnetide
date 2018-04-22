using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class ExpressionEvaluationTests : ExpressionTestBed
    {
        [TestMethod]
        public void UnknowSymbolEvaluatesToNull()
        {
            EvalExpression("UNKNOWN", null);
        }

        [TestMethod]
        [DataRow("KNOWN", 0x23EA)]
        [DataRow("known", 0x23EA)]
        public void KnowSymbolEvaluatesToItsValue(string source, int expected)
        {
            var symbols = new Dictionary<string, ushort>
            {
                { "known", 0x23EA },
                { "other", 0xDD34 },
            };
            EvalExpression(source, (ushort)expected, symbols: symbols);
        }

        [TestMethod]
        [DataRow("+0", 0)]
        [DataRow("+12345", 12345)]
        [DataRow("+99999", 34463)]
        [DataRow("+#12AC", 0x12AC)]
        public void UnaryPlusWorksWithIntegerAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("+true", 1)]
        [DataRow("+false", 0)]
        public void UnaryPlusWorksWithBoolAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("+0.0", 0.0)]
        [DataRow("+3.14", 3.14)]
        [DataRow("+.25", 0.25)]
        [DataRow("+3.14E2", 3.14E2)]
        [DataRow("+3.14E+2", 3.14E+2)]
        [DataRow("+3.14E-2", 3.14E-2)]
        [DataRow("+1E8", 1E8)]
        [DataRow("+2E+8", 2E8)]
        [DataRow("+3E-8", 3E-8)]
        [DataRow("+3E-188888", 0.0)]
        public void UnaryPlusWorksWithRealAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("-0", 0)]
        [DataRow("-12345", -12345)]
        [DataRow("-99999", -34463)]
        [DataRow("-#12AC", -0x12AC)]
        public void UnaryMinusWorksWithIntegerAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("-true", -1)]
        [DataRow("-false", 0)]
        public void UnaryMinusWorksWithBoolAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("-0.0", 0.0)]
        [DataRow("-3.14", -3.14)]
        [DataRow("-.25", -0.25)]
        [DataRow("-3.14E2", -3.14E2)]
        [DataRow("-3.14E+2", -3.14E+2)]
        [DataRow("-3.14E-2", -3.14E-2)]
        [DataRow("-1E8", -1E8)]
        [DataRow("-2E+8", -2E8)]
        [DataRow("-3E-8", -3E-8)]
        [DataRow("-3E-188888", 0.0)]
        public void UnaryMinusWorksWithRealAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("~0", 0xffff)]
        [DataRow("~#aa55", 0x55aa)]
        public void UnaryBitwiseNotWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("~true", 0xfffe)]
        [DataRow("~false", 0xffff)]
        public void UnaryBitwiseNotWithBoolWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("~3.14")]
        [DataRow("~\"abc\"")]
        public void UnaryBitwiseNotFailsWithRealAndString(string source)
        {
            EvalFails(source);
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
    }
}
