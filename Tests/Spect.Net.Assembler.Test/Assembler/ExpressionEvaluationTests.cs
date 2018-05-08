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
            RemainsUnevaluated("UNKNOWN");
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
        [DataRow("!0", 0x0001)]
        [DataRow("!#aa55", 0x0000)]
        public void UnaryLogicalNotWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("!true", 0x0000)]
        [DataRow("!false", 0x0001)]
        public void UnaryLogicalNotWithBoolWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("!3.14")]
        [DataRow("!\"abc\"")]
        public void UnaryLogicalNotFailsWithRealAndString(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 + 3", 3)]
        [DataRow("12 + 23", 35)]
        [DataRow("#8000 + #4000", 0xC000)]
        public void AdditionWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false + false", 0)]
        [DataRow("false + true", 1)]
        [DataRow("true + false", 1)]
        [DataRow("true + true", 2)]
        public void AdditionWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false + 123", 123)]
        [DataRow("123 + false", 123)]
        [DataRow("true + 123", 124)]
        [DataRow("123 + true", 124)]
        public void AdditionWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 + 3.14", 3.14)]
        [DataRow("1e1 + 2e1", 3e1)]
        [DataRow("1.2 + 3.14e-1", 1.514)]
        public void AdditionWithRealWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 + 3", 3.0)]
        [DataRow("1e1 + 20", 3e1)]
        [DataRow("1.2 + 1", 2.2)]
        [DataRow("3 + 0.0", 3.0)]
        [DataRow("20 + 1e1", 3e1)]
        [DataRow("1 + 1.2", 2.2)]
        public void AdditionWithRealAndIntWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 + false", 0.0)]
        [DataRow("1e1 + true", 11.0)]
        [DataRow("1.2 + true", 2.2)]
        [DataRow("false + 0.0", 0.0)]
        [DataRow("true + 1e1", 11)]
        [DataRow("false + 1.2", 1.2)]
        public void AdditionWithRealAndBoolWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"abc\" + \"def\"", "abcdef")]
        [DataRow("\"abc\" + \"\"", "abc")]
        [DataRow("\"\" + \"def\"", "def")]
        public void AdditionWithStringWorkAsExpected(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true + \"abc\"")]
        [DataRow("1 + \"abc\"")]
        [DataRow("1.1 + \"abc\"")]
        [DataRow("\"abc\" + false")]
        [DataRow("\"abc\" + 1")]
        [DataRow("\"abc\" + 1.1")]
        public void AdditionFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 - 3", -3)]
        [DataRow("23 - 12", 11)]
        [DataRow("#8000 - #4000", 0x4000)]
        public void SubtractionWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false - false", 0)]
        [DataRow("false - true", -1)]
        [DataRow("true - false", 1)]
        [DataRow("true - true", 0)]
        public void SubtractionWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false - 123", -123)]
        [DataRow("123 - false", 123)]
        [DataRow("true - 123", -122)]
        [DataRow("123 - true", 122)]
        public void SubtractionWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 - 3.14", -3.14)]
        [DataRow("1e1 - 2e1", -1e1)]
        [DataRow("1.2 - 3.14e-1", 0.886)]
        public void SubtractionWithRealWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 - 3", -3.0)]
        [DataRow("1e1 - 20", -1e1)]
        [DataRow("1.2 - 1", 0.2)]
        [DataRow("3 - 0.0", 3.0)]
        [DataRow("20 - 1e1", 10)]
        [DataRow("1 - 1.2", -0.2)]
        public void SubtractionWithRealAndIntWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 - false", 0.0)]
        [DataRow("1e1 - true", 9.0)]
        [DataRow("1.2 - true", 0.2)]
        [DataRow("false - 0.0", 0.0)]
        [DataRow("true - 1e1", -9.0)]
        [DataRow("false - 1.2", -1.2)]
        public void SubtractionWithRealAndBoolWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true - \"abc\"")]
        [DataRow("1 - \"abc\"")]
        [DataRow("1.1 - \"abc\"")]
        [DataRow("\"abc\" - false")]
        [DataRow("\"abc\" - 1")]
        [DataRow("\"abc\" - 1.1")]
        public void SubtractionFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 * 3", -0)]
        [DataRow("23 * 12", 276)]
        [DataRow("#8000 - #4000", 0x4000)]
        public void MultiplicationWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false - false", 0)]
        [DataRow("false - true", -1)]
        [DataRow("true - false", 1)]
        [DataRow("true - true", 0)]
        public void MultiplicationWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false * 123", 0)]
        [DataRow("123 * false", 0)]
        [DataRow("true * 123", 123)]
        [DataRow("123 * true", 123)]
        public void MultiplicationWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 * 3.14", 0.0)]
        [DataRow("1e1 * 2e1", 2e2)]
        [DataRow("1.2 * 3.14e-1", 0.3768)]
        public void MultiplicationWithRealWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 * 3", 0.0)]
        [DataRow("1e1 * 20", 200.0)]
        [DataRow("1.2 * 1", 1.2)]
        [DataRow("3 * 0.0", 0.0)]
        [DataRow("20 * 1e1", 200.0)]
        [DataRow("1 * 1.2", 1.2)]
        public void MultiplicationWithRealAndIntWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 * false", 0.0)]
        [DataRow("1e1 * true", 1e1)]
        [DataRow("1.2 * true", 1.2)]
        [DataRow("false * 0.0", 0.0)]
        [DataRow("true * 1e1", 1e1)]
        [DataRow("false * 1.2", 0.0)]
        public void MultiplicationWithRealAndBoolWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true * \"abc\"")]
        [DataRow("1 * \"abc\"")]
        [DataRow("1.1 * \"abc\"")]
        [DataRow("\"abc\" * false")]
        [DataRow("\"abc\" * 1")]
        [DataRow("\"abc\" * 1.1")]
        public void MultiplicationFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 / 3", 0)]
        [DataRow("23 / 12", 1)]
        [DataRow("#8000 / #4000", 2)]
        public void DivisionWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false / true", 0)]
        [DataRow("true / true", 1)]
        public void DivisionWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false / 123", 0)]
        [DataRow("true / 123", 0)]
        [DataRow("123 / true", 123)]
        public void DivisionWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 / 3.14", 0.0)]
        [DataRow("1e1 / 2e1", 0.5)]
        [DataRow("3.9 / 3.25e-1", 12.0)]
        public void DivisionWithRealWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.0 / 3", 0.0)]
        [DataRow("1e1 / 20", 0.5)]
        [DataRow("1.2 / 1", 1.2)]
        [DataRow("20 / 1e1", 2.0)]
        [DataRow("1 / 1.25", 0.8)]
        public void DivisionWithRealAndIntWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1e1 / true", 1e1)]
        [DataRow("1.2 / true", 1.2)]
        [DataRow("true / 1e1", 0.1)]
        [DataRow("false / 1.2", 0.0)]
        public void DivisionWithRealAndBoolWorkAsExpected(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true / \"abc\"")]
        [DataRow("1 / \"abc\"")]
        [DataRow("1.1 / \"abc\"")]
        [DataRow("\"abc\" / false")]
        [DataRow("\"abc\" / 1")]
        [DataRow("\"abc\" / 1.1")]
        public void DivisionFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("112 / 0")]
        [DataRow("112.0 / 0")]
        [DataRow("112 / false")]
        [DataRow("112.0 / false")]
        [DataRow("112 / 0.0")]
        [DataRow("112.0 / 0.0")]
        public void DivideByZeroRaisesError(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 % 3", 0)]
        [DataRow("23 % 12", 11)]
        [DataRow("#8000 % #4000", 0)]
        public void ModuloWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false % true", 0)]
        [DataRow("true % true", 0)]
        public void ModuloWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false % 123", 0)]
        [DataRow("true % 123", 1)]
        [DataRow("123 % true", 0)]
        public void ModuloWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true % \"abc\"")]
        [DataRow("1 % \"abc\"")]
        [DataRow("1.1 % \"abc\"")]
        [DataRow("\"abc\" % false")]
        [DataRow("\"abc\" % 1")]
        [DataRow("\"abc\" % 1.1")]
        [DataRow("112.0 % 2")]
        [DataRow("112 % 2.0")]
        [DataRow("112.0 % true")]
        public void ModuloFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("112 % 0")]
        [DataRow("112.0 % 0")]
        [DataRow("112 % false")]
        [DataRow("112.0 % false")]
        [DataRow("112 % 0.0")]
        [DataRow("112.0 % 0.0")]
        public void ModuloByZeroRaisesError(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 & 3", 0)]
        [DataRow("23 & 12", 4)]
        [DataRow("#8000 & #4000", 0)]
        public void BitwiseAndWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false & true", 0)]
        [DataRow("true & true", 1)]
        [DataRow("false & false", 0)]
        [DataRow("true & false", 0)]
        public void BitwiseAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false & 123", 0)]
        [DataRow("true & 123", 1)]
        [DataRow("123 & true", 1)]
        public void BitwiseAndWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true & \"abc\"")]
        [DataRow("1 & \"abc\"")]
        [DataRow("1.1 & \"abc\"")]
        [DataRow("\"abc\" & false")]
        [DataRow("\"abc\" & 1")]
        [DataRow("\"abc\" & 1.1")]
        [DataRow("112.0 & 2")]
        [DataRow("112 & 2.0")]
        [DataRow("112.0 & true")]
        public void BitwiseAndFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 | 3", 3)]
        [DataRow("23 | 12", 31)]
        [DataRow("#8000 | #4000", 0xC000)]
        public void BitwiseOrWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false | true", 1)]
        [DataRow("true | true", 1)]
        [DataRow("false | false", 0)]
        [DataRow("true | false", 1)]
        public void BitwiseOrBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false | 123", 123)]
        [DataRow("true | 123", 123)]
        [DataRow("122 | true", 123)]
        [DataRow("122| false", 122)]
        public void BitwiseOrWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true | \"abc\"")]
        [DataRow("1 | \"abc\"")]
        [DataRow("1.1 | \"abc\"")]
        [DataRow("\"abc\" | false")]
        [DataRow("\"abc\" | 1")]
        [DataRow("\"abc\" | 1.1")]
        [DataRow("112.0 | 2")]
        [DataRow("112 | 2.0")]
        [DataRow("112.0 | true")]
        public void BitwiseOrFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 ^ 3", 3)]
        [DataRow("23 ^ 13", 26)]
        [DataRow("#8100 ^ #4100", 0xC000)]
        public void BitwiseXorWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false ^ true", 1)]
        [DataRow("true ^ true", 0)]
        [DataRow("false ^ false", 0)]
        [DataRow("true ^ false", 1)]
        public void BitwiseXorBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false ^ 123", 123)]
        [DataRow("true ^ 123", 122)]
        [DataRow("122 ^ true", 123)]
        [DataRow("122 ^ false", 122)]
        public void BitwiseXorWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true ^ \"abc\"")]
        [DataRow("1 ^ \"abc\"")]
        [DataRow("1.1 ^ \"abc\"")]
        [DataRow("\"abc\" ^ false")]
        [DataRow("\"abc\" ^ 1")]
        [DataRow("\"abc\" ^ 1.1")]
        [DataRow("112.0 ^ 2")]
        [DataRow("112 ^ 2.0")]
        [DataRow("112.0 ^ true")]
        public void BitwiseXorFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
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
        [DataRow("0 == 0", 1)]
        [DataRow("0 == 1", 0)]
        [DataRow("23 == 20+3", 1)]
        [DataRow("23-3 == 20+3", 0)]
        public void EqualityWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false == false", 1)]
        [DataRow("false == true", 0)]
        [DataRow("true == false", 0)]
        [DataRow("true == true", 1)]
        public void EqualityWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.1 == 0.1", 1)]
        [DataRow("1e1 == 3.14", 0)]
        [DataRow("3.14e1 == 0.25", 0)]
        [DataRow("3e4 == 3e4", 1)]
        public void EqualityWithRealWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 == 1", 1)]
        [DataRow("1e1 == 9", 0)]
        [DataRow("1 == 1e1", 0)]
        [DataRow("10 == 1e1", 1)]
        public void EqualityWithRealAndIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 == true", 1)]
        [DataRow("0.0 == false", 1)]
        [DataRow("1e1 == false", 0)]
        [DataRow("1e1 == true", 0)]
        [DataRow("true == 1.0", 1)]
        [DataRow("false == 0.0", 1)]
        [DataRow("false == 1e1", 0)]
        [DataRow("true == 1e1", 0)]
        public void EqualityWithRealAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"abc\" == \"def\"", 0)]
        [DataRow("\"abc\" == \"abc\"", 1)]
        [DataRow("\"\" == \"def\"", 0)]
        [DataRow("\"\" == \"\"", 1)]
        public void EqualityWithStringWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true == \"abc\"")]
        [DataRow("1 == \"abc\"")]
        [DataRow("1.1 == \"abc\"")]
        [DataRow("\"abc\" == false")]
        [DataRow("\"abc\" == 1")]
        [DataRow("\"abc\" == 1.1")]
        public void EqualityFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 != 0", 0)]
        [DataRow("0 != 1", 1)]
        [DataRow("23 != 20+3", 0)]
        [DataRow("23-3 != 20+3", 1)]
        public void UnequalityWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false != false", 0)]
        [DataRow("false != true", 1)]
        [DataRow("true != false", 1)]
        [DataRow("true != true", 0)]
        public void UnequalityWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.1 != 0.1", 0)]
        [DataRow("1e1 != 3.14", 1)]
        [DataRow("3.14e1 != 0.25", 1)]
        [DataRow("3e4 != 3e4", 0)]
        public void UnequalityWithRealWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 != 1", 0)]
        [DataRow("1e1 != 9", 1)]
        [DataRow("1 != 1e1", 1)]
        [DataRow("10 != 1e1", 0)]
        public void UnequalityWithRealAndIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 != true", 0)]
        [DataRow("0.0 != false", 0)]
        [DataRow("1e1 != false", 1)]
        [DataRow("1e1 != true", 1)]
        [DataRow("true != 1.0", 0)]
        [DataRow("false != 0.0", 0)]
        [DataRow("false != 1e1", 1)]
        [DataRow("true != 1e1", 1)]
        public void UneualityWithRealAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"abc\" != \"def\"", 1)]
        [DataRow("\"abc\" != \"abc\"", 0)]
        [DataRow("\"\" != \"def\"", 1)]
        [DataRow("\"\" != \"\"", 0)]
        public void UnequalityWithStringWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true != \"abc\"")]
        [DataRow("1 != \"abc\"")]
        [DataRow("1.1 != \"abc\"")]
        [DataRow("\"abc\" != false")]
        [DataRow("\"abc\" != 1")]
        [DataRow("\"abc\" != 1.1")]
        public void UnequalityFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 > 0", 0)]
        [DataRow("0 > 1", 0)]
        [DataRow("23 > 20", 1)]
        [DataRow("23+3 > 20+3", 1)]
        public void GreaterThanWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false > false", 0)]
        [DataRow("false > true", 0)]
        [DataRow("true > false", 1)]
        [DataRow("true > true", 0)]
        public void GreaterThanWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.1 > 0.1", 0)]
        [DataRow("1e1 > 3.14", 1)]
        [DataRow("3.14e1 > 0.25", 1)]
        [DataRow("3e4 > 3e4", 0)]
        public void GreaterThanWithRealWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 > 1", 0)]
        [DataRow("1e1 > 9", 1)]
        [DataRow("1 > 1e1", 0)]
        [DataRow("10 > 1e1", 0)]
        public void GreaterThanWithRealAndIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 > true", 0)]
        [DataRow("0.0 > false", 0)]
        [DataRow("1e1 > false", 1)]
        [DataRow("1e1 > true", 1)]
        [DataRow("true > 1.0", 0)]
        [DataRow("false > 0.0", 0)]
        [DataRow("false > 1e1", 0)]
        [DataRow("true > 1e1", 0)]
        public void GreaterThanWithRealAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"def\" > \"abc\"", 1)]
        [DataRow("\"abc\" > \"abc\"", 0)]
        [DataRow("\"\" > \"def\"", 0)]
        [DataRow("\"\" > \"\"", 0)]
        public void GreaterThanWithStringWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true > \"abc\"")]
        [DataRow("1 > \"abc\"")]
        [DataRow("1.1 > \"abc\"")]
        [DataRow("\"abc\" > false")]
        [DataRow("\"abc\" > 1")]
        [DataRow("\"abc\" > 1.1")]
        public void GreaterThanFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 >= 0", 1)]
        [DataRow("0 >= 1", 0)]
        [DataRow("23 >= 20", 1)]
        [DataRow("23+3 >= 20+3", 1)]
        public void GreaterThanOrEqualWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false >= false", 1)]
        [DataRow("false >= true", 0)]
        [DataRow("true >= false", 1)]
        [DataRow("true >= true", 1)]
        public void GreaterThanOrEqualWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.1 >= 0.1", 1)]
        [DataRow("1e1 >= 3.14", 1)]
        [DataRow("3.14e1 >= 0.25", 1)]
        [DataRow("3e4 >= 3e4", 1)]
        [DataRow("1e4 >= 3e4", 0)]
        public void GreaterThanOrEqualWithRealWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 >= 1", 1)]
        [DataRow("1e1 >= 9", 1)]
        [DataRow("1 >= 1e1", 0)]
        [DataRow("10 >= 1e1", 1)]
        public void GreaterThanOrEqualWithRealAndIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 >= true", 1)]
        [DataRow("0.0 >= false", 1)]
        [DataRow("1e1 >= false", 1)]
        [DataRow("1e1 >= true", 1)]
        [DataRow("true >= 1.0", 1)]
        [DataRow("false >= 0.0", 1)]
        [DataRow("false >= 1e1", 0)]
        [DataRow("true >= 1e1", 0)]
        public void GreaterThanOrEqualWithRealAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"def\" >= \"abc\"", 1)]
        [DataRow("\"abc\" >= \"abc\"", 1)]
        [DataRow("\"\" >= \"def\"", 0)]
        [DataRow("\"\" >= \"\"", 1)]
        public void GreaterThanOrEqualWithStringWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true >= \"abc\"")]
        [DataRow("1 >= \"abc\"")]
        [DataRow("1.1 >= \"abc\"")]
        [DataRow("\"abc\" >= false")]
        [DataRow("\"abc\" >= 1")]
        [DataRow("\"abc\" >= 1.1")]
        public void GreaterThanOrEqualFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 < 0", 0)]
        [DataRow("0 < 1", 1)]
        [DataRow("23 < 20", 0)]
        [DataRow("23+3 < 20+3", 0)]
        public void LessThanWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false < false", 0)]
        [DataRow("false < true", 1)]
        [DataRow("true < false", 0)]
        [DataRow("true < true", 0)]
        public void LessThanWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.1 < 0.1", 0)]
        [DataRow("1e1 < 3.14", 0)]
        [DataRow("3.14e1 < 0.25", 0)]
        [DataRow("3e4 < 3e4", 0)]
        [DataRow("2e4 < 3e4", 1)]
        public void LessThanWithRealWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 < 1", 0)]
        [DataRow("1e1 < 9", 0)]
        [DataRow("1 < 1e1", 1)]
        [DataRow("10 < 1e1", 0)]
        public void LessThanWithRealAndIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 < true", 0)]
        [DataRow("0.0 < false", 0)]
        [DataRow("1e1 < false", 0)]
        [DataRow("1e1 < true", 0)]
        [DataRow("true < 1.0", 0)]
        [DataRow("false < 0.0", 0)]
        [DataRow("false < 1e1", 1)]
        [DataRow("true < 1e1", 1)]
        public void LessThanWithRealAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"def\" < \"abc\"", 0)]
        [DataRow("\"abc\" < \"abc\"", 0)]
        [DataRow("\"abc\" < \"def\"", 1)]
        [DataRow("\"\" < \"def\"", 1)]
        [DataRow("\"\" < \"\"", 0)]
        public void LessThanWithStringWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true < \"abc\"")]
        [DataRow("1 < \"abc\"")]
        [DataRow("1.1 < \"abc\"")]
        [DataRow("\"abc\" < false")]
        [DataRow("\"abc\" < 1")]
        [DataRow("\"abc\" < 1.1")]
        public void LessThanFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("0 <= 0", 1)]
        [DataRow("0 <= 1", 1)]
        [DataRow("23 <= 20", 0)]
        [DataRow("23+3 <= 20+3", 0)]
        public void LessThanOrEqualWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("false <= false", 1)]
        [DataRow("false <= true", 1)]
        [DataRow("true <= false", 0)]
        [DataRow("true <= true", 1)]
        public void LessThanOrEqualWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("0.1 <= 0.1", 1)]
        [DataRow("1e1 <= 3.14", 0)]
        [DataRow("3.14e1 <= 0.25", 0)]
        [DataRow("3e4 <= 3e4", 1)]
        [DataRow("2e4 <= 3e4", 1)]
        public void LessThanOrEqualWithRealWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 <= 1", 1)]
        [DataRow("1e1 <= 9", 0)]
        [DataRow("1 <= 1e1", 1)]
        [DataRow("10 <= 1e1", 1)]
        public void LessThanOrEqualWithRealAndIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("1.0 <= true", 1)]
        [DataRow("0.0 <= false", 1)]
        [DataRow("1e1 <= false", 0)]
        [DataRow("1e1 <= true", 0)]
        [DataRow("true <= 1.0", 1)]
        [DataRow("false <= 0.0", 1)]
        [DataRow("false <= 1e1", 1)]
        [DataRow("true <= 1e1", 1)]
        public void LessThanOrEqualWithRealAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("\"def\" <= \"abc\"", 0)]
        [DataRow("\"abc\" <= \"abc\"", 1)]
        [DataRow("\"abc\" <= \"def\"", 1)]
        [DataRow("\"\" <= \"def\"", 1)]
        [DataRow("\"\" <= \"\"", 1)]
        public void LessThanOrEqualWithStringWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("true <= \"abc\"")]
        [DataRow("1 <= \"abc\"")]
        [DataRow("1.1 <= \"abc\"")]
        [DataRow("\"abc\" <= false")]
        [DataRow("\"abc\" <= 1")]
        [DataRow("\"abc\" <= 1.1")]
        public void LessThanOrEqualFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("1 << 8", 256)]
        [DataRow("3  << 4", 48)]
        [DataRow("#FFFF << 0", 0xFFFF)]
        [DataRow("#FFFF << 12", 0xF000)]
        [DataRow("12345 << 15", 0x8000)]
        public void ShiftLeftWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("false << false", 0)]
        [DataRow("false << true", 0)]
        [DataRow("true << false", 1)]
        [DataRow("true << true", 2)]
        public void ShiftLeftWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("#30 << false", 0x30)]
        [DataRow("#30 << true", 0x60)]
        [DataRow("true << 2", 4)]
        [DataRow("false << 2", 0)]
        public void ShiftLeftWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("true << \"abc\"")]
        [DataRow("1 << \"abc\"")]
        [DataRow("1.1 << \"abc\"")]
        [DataRow("\"abc\" << false")]
        [DataRow("\"abc\" << 1")]
        [DataRow("\"abc\" << 1.1")]
        [DataRow("1 << 1.1")]
        [DataRow("1.1 << 1")]
        public void ShiftLeftFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("256 >> 8", 1)]
        [DataRow("48  >> 4", 3)]
        [DataRow("#FFFF >> 0", 0xFFFF)]
        [DataRow("#FFFF >> 12", 0x000F)]
        [DataRow("32800 >> 15", 0x0001)]
        [DataRow("32800 >> 24", 0x0000)]
        public void ShiftRightWithIntWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("false >> false", 0)]
        [DataRow("false >> true", 0)]
        [DataRow("true >> false", 1)]
        [DataRow("true >> true", 0)]
        public void ShiftRightWithBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("#30 >> false", 0x30)]
        [DataRow("#30 >> true", 0x18)]
        [DataRow("true > 2", 0)]
        [DataRow("false >> 2", 0)]
        public void ShiftRightWithIntAndBoolWorkAsExpected(string source, int expected)
        {
            EvalExpression(source, (ushort)expected);
        }

        [TestMethod]
        [DataRow("true >> \"abc\"")]
        [DataRow("1 >> \"abc\"")]
        [DataRow("1.1 >> \"abc\"")]
        [DataRow("\"abc\" >> false")]
        [DataRow("\"abc\" >> 1")]
        [DataRow("\"abc\" >> 1.1")]
        [DataRow("1 >> 1.1")]
        [DataRow("1.1 >> 1")]
        public void ShiftRightFailsWithIncompatibleTypes(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        public void ConditionalOpsWorkAsExpected()
        {
            EvalExpression("23+11 > 3 ? 123 : 456", 123);
            EvalExpression("23+11 < 3 ? 123 : 456", 456);
        }
    }
}
