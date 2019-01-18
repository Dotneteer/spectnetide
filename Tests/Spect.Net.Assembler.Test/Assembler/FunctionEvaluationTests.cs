using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class FunctionEvaluationTests : ExpressionTestBed
    {
        [TestMethod]
        [DataRow("abs(false)", 0)]
        [DataRow("abs(true)", 1)]
        [DataRow("abs(-true)", 1)]
        public void AbsWithBoolWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("abs(-123)", 123)]
        [DataRow("abs(0)", 0)]
        [DataRow("abs(123)", 123)]
        public void AbsWithIntegerWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("abs(-123.25)", 123.25)]
        [DataRow("abs(0.0)", 0.0)]
        [DataRow("abs(123.25)", 123.25)]
        public void AbsWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("abs(\"fail\")")]
        [DataRow("abs(\"\")")]
        public void AbsWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("acos(false)", 1.5707963267948966)]
        [DataRow("acos(true)", 0.0)]
        public void AcosWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("acos(0)", 1.5707963267948966)]
        [DataRow("acos(1)", 0.0)]
        public void AcosWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("acos(0.0)", 1.5707963267948966)]
        [DataRow("acos(1.0)", 0.0)]
        public void AcosWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("acos(\"fail\")")]
        [DataRow("acos(\"\")")]
        public void AcosWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("asin(true)", 1.5707963267948966)]
        [DataRow("asin(false)", 0.0)]
        public void AsinWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("asin(1)", 1.5707963267948966)]
        [DataRow("asin(0)", 0.0)]
        public void AsinWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("asin(1.0)", 1.5707963267948966)]
        [DataRow("asin(0.0)", 0.0)]
        public void AsinWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("asin(\"fail\")")]
        [DataRow("asin(\"\")")]
        public void AsinWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("atan(true)", 0.78539816339744828)]
        [DataRow("atan(false)", 0.0)]
        public void AtanWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("atan(1)", 0.78539816339744828)]
        [DataRow("atan(0)", 0.0)]
        public void AtanWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("atan(1.0)", 0.78539816339744828)]
        [DataRow("atan(0.0)", 0.0)]
        public void AtanWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("atan(\"fail\")")]
        [DataRow("atan(\"\")")]
        public void AtanWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("atan2(true, true)", 0.78539816339744828)]
        [DataRow("atan2(false, true)", 0.0)]
        public void Atan2WithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("atan2(1, 1)", 0.78539816339744828)]
        [DataRow("atan2(0, 1)", 0.0)]
        public void Atan2WithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("atan2(1.0, 1)", 0.78539816339744828)]
        [DataRow("atan2(0.0, 1.0)", 0.0)]
        public void Atan2WithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("atan2(\"fail\")")]
        [DataRow("atan2(\"\")")]
        public void Atan2WithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("ceiling(true)", 1)]
        [DataRow("ceiling(false)", 0)]
        public void CeilingWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("ceiling(1)", 1)]
        [DataRow("ceiling(0)", 0)]
        public void CeilingWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("ceiling(1.0)", 1.0)]
        [DataRow("ceiling(0.0)", 0.0)]
        [DataRow("ceiling(1.1)", 2.0)]
        public void CeilingWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("ceiling(\"fail\")")]
        [DataRow("ceiling(\"\")")]
        public void CeilingWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("cos(true)", 0.54030230586813976)]
        [DataRow("cos(false)", 1.0)]
        public void CosWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("cos(1)", 0.54030230586813976)]
        [DataRow("cos(0)", 1.0)]
        public void CosWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("cos(1.0)", 0.54030230586813976)]
        [DataRow("cos(0.0)", 1.0)]
        public void CosWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("cos(\"fail\")")]
        [DataRow("cos(\"\")")]
        public void CosWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("cosh(true)", 1.5430806348152437)]
        [DataRow("cosh(false)", 1.0)]
        public void CoshWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("cosh(1)", 1.5430806348152437)]
        [DataRow("cosh(0)", 1.0)]
        public void CoshWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("cosh(1.0)", 1.5430806348152437)]
        [DataRow("cosh(0.0)", 1.0)]
        public void CoshWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("cosh(\"fail\")")]
        [DataRow("cosh(\"\")")]
        public void CoshWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("exp(true)", 2.7182818284590451)]
        [DataRow("exp(false)", 1.0)]
        public void ExpWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("exp(1)", 2.7182818284590451)]
        [DataRow("exp(0)", 1.0)]
        public void ExpWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("exp(1.0)", 2.7182818284590451)]
        [DataRow("exp(0.0)", 1.0)]
        public void ExpWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("exp(\"fail\")")]
        [DataRow("exp(\"\")")]
        public void ExpWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("floor(true)", 1)]
        [DataRow("floor(false)", 0)]
        public void FloorWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("floor(1)", 1)]
        [DataRow("floor(0)", 0)]
        public void FloorWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("floor(1.0)", 1.0)]
        [DataRow("floor(0.0)", 0.0)]
        [DataRow("floor(1.1)", 1.0)]
        public void FloorWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("floor(\"fail\")")]
        [DataRow("floor(\"\")")]
        public void FloorWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("log(true)", 0)]
        public void LogWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log(1)", 0)]
        [DataRow("log(2)", 0.69314718055994529)]
        public void LogWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log(1.0)", 0)]
        [DataRow("log(2.0)", 0.69314718055994529)]
        public void LogWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log(\"fail\")")]
        [DataRow("log(\"\")")]
        public void LogWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("log(true, true)", 0)]
        public void Log2WithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log(100, 10)", 2)]
        [DataRow("log(1000, 10)", 3)]
        public void Log2WithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log(100, 10)", 2)]
        [DataRow("log(1000, 10)", 3)]
        public void Log2WithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log(\"fail\", true)")]
        [DataRow("log(\"\", \"\")")]
        public void Log2WithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("log10(true)", 0.0)]
        public void Log10WithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log10(100)", 2)]
        [DataRow("log10(1000)", 3)]
        public void Log10WithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log10(100)", 2)]
        [DataRow("log10(1000)", 3)]
        public void Log10WithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("log10(\"fail\", true)")]
        [DataRow("log10(\"\", \"\")")]
        public void Log10WithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("max(true, false)", 1)]
        [DataRow("max(false, false)", 0)]
        public void MaxWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("max(1, 0)", 1)]
        [DataRow("max(0, -1)", 0)]
        public void MaxWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("max(1.0, 0.0)", 1)]
        [DataRow("max(0.0, -1)", 0)]
        public void MaxWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("max(\"fail\", true)")]
        [DataRow("max(\"\", \"\")")]
        public void MaxWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("min(true, false)", 0)]
        [DataRow("min(false, false)", 0)]
        public void MinWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("min(1, 0)", 0)]
        [DataRow("min(0, -1)", -1)]
        public void MinWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("min(1.0, 0.0)", 0.0)]
        [DataRow("min(0.0, -1)", -1.0)]
        public void MinWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("min(\"fail\", true)")]
        [DataRow("min(\"\", \"\")")]
        public void MinWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("Pow(true, false)", 1)]
        [DataRow("Pow(true, true)", 1)]
        public void PowWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("Pow(2, 3)", 8)]
        [DataRow("Pow(3, 4)", 81)]
        public void PowWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("Pow(2.0, 3)", 8.0)]
        [DataRow("Pow(3.0, 4.0)", 81.0)]
        public void PowWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("pow(\"fail\", true)")]
        [DataRow("pow(\"\", \"\")")]
        public void PowWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("round(true)", 1)]
        [DataRow("round(false)", 0)]
        public void RoundWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("round(2)", 2)]
        [DataRow("round(3)", 3)]
        public void RoundWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("round(2.0)", 2)]
        [DataRow("round(3.0)", 3)]
        [DataRow("round(3.25)", 3)]
        [DataRow("round(3.75)", 4)]
        public void RoundWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("round(\"fail\")")]
        [DataRow("round(\"\")")]
        public void RoundWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("round(true, true)", 1)]
        [DataRow("round(false, false)", 0)]
        public void Round2WithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("round(2, 1)", 2)]
        [DataRow("round(3, 2)", 3)]
        public void Round2WithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("round(2.0, 1)", 2)]
        [DataRow("round(3.0, 1)", 3)]
        [DataRow("round(3.26, 1)", 3.3)]
        [DataRow("round(3.75, 2)", 3.75)]
        public void Round2WithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("round(\"fail\", true)")]
        [DataRow("round(\"\", \"\")")]
        public void Round2WithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("sign(true)", 1)]
        [DataRow("sign(false)", 0)]
        public void SignWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sign(2)", 1)]
        [DataRow("sign(0)", 0)]
        [DataRow("sign(-2)", -1)]
        public void SignWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sign(2.0)", 1.0)]
        [DataRow("sign(0.0)", 0.0)]
        [DataRow("sign(-2.0)", -1.0)]
        public void SignWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sign(\"fail\")")]
        [DataRow("sign(\"\")")]
        public void SignWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("sin(true)", 0.8414709848078965)]
        [DataRow("sin(false)", 0.0)]
        public void SinWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sin(\"fail\")")]
        [DataRow("sin(\"\")")]
        public void SinWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("sinh(true)", 1.1752011936438014)]
        [DataRow("sinh(false)", 0.0)]
        public void SinhWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sinh(1)", 1.1752011936438014)]
        [DataRow("sinh(0)", 0.0)]
        public void SinhWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sinh(1.0)", 1.1752011936438014)]
        [DataRow("sinh(0.0)", 0.0)]
        public void SinhWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sinh(\"fail\")")]
        [DataRow("sinh(\"\")")]
        public void SinhWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("sqrt(true)", 1)]
        [DataRow("sqrt(false)", 0)]
        public void SqrtWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sqrt(1)", 1.0)]
        [DataRow("sqrt(0)", 0.0)]
        public void SqrtWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sqrt(2.25)", 1.5)]
        [DataRow("sqrt(4.0)", 2.0)]
        public void SqrtWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("sqrt(\"fail\")")]
        [DataRow("sqrt(\"\")")]
        public void SqrtWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("tan(true)", 1.5574077246549023)]
        [DataRow("tan(false)", 0)]
        public void TanWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("tan(1)", 1.5574077246549023)]
        [DataRow("tan(0)", 0)]
        public void TanWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("tan(1.0)", 1.5574077246549023)]
        [DataRow("tan(0.0)", 0)]
        public void TanWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("tan(\"fail\")")]
        [DataRow("tan(\"\")")]
        public void TanWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("tanh(true)", 0.76159415595576485)]
        [DataRow("tanh(false)", 0)]
        public void TanhWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("tanh(1)", 0.76159415595576485)]
        [DataRow("tanh(0)", 0)]
        public void TanhWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("tanh(1.0)", 0.76159415595576485)]
        [DataRow("tanh(0.0)", 0)]
        public void TanhWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("tanh(\"fail\")")]
        [DataRow("tanh(\"\")")]
        public void TanhWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("truncate(true)", 1)]
        [DataRow("truncate(false)", 0)]
        public void TruncateWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("truncate(1)", 1)]
        [DataRow("truncate(0)", 0)]
        public void TruncateWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("truncate(1.0)", 1.0)]
        [DataRow("truncate(0.0)", 0.0)]
        [DataRow("truncate(1.1)", 1.0)]
        [DataRow("truncate(1.9)", 1.0)]
        public void TruncateWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("truncate(\"fail\")")]
        [DataRow("truncate(\"\")")]
        public void TruncateWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("pi()", 3.1415926535897931)]
        public void PiWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("nat()", 2.7182818284590451)]
        public void NatWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("low(false)", 0)]
        [DataRow("low(true)", 1)]
        public void LowWithBoolWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("low(#fe)", 0xfe)]
        [DataRow("low(#fe*#fe)", 0x04)]
        [DataRow("low(#1234*#1234)", 0x90)]
        public void LowWithIntegerWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("low(254.0)")]
        [DataRow("low(254.1*254.1)")]
        public void LowWithRealFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("low(\"\")")]
        [DataRow("low(\"abc\")")]
        public void LowWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("low(truncate(254.0))", 254)]
        [DataRow("low(truncate(254.1*254.1))", 0x36)]
        public void LowWithRealAndTruncateWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("high(false)", 0)]
        [DataRow("high(true)", 0)]
        public void HighWithBoolWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("high(#fe13)", 0xFE)]
        [DataRow("high(#fe*#fe)", 0xFC)]
        [DataRow("high(#1234*#1234)", 0x5A)]
        public void HighWithIntegerWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("high(254.0)")]
        [DataRow("high(254.1*254.1)")]
        public void HighWithRealFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("high(\"\")")]
        [DataRow("high(\"abc\")")]
        public void HighWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("high(truncate(25400.0))", 0x63)]
        [DataRow("high(truncate(254.1*254.1))", 0xFC)]
        public void HighWithRealAndTruncateWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("word(false)", 0)]
        [DataRow("word(true)", 1)]
        public void WordWithBoolWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("word(#fe13*#13)", 0xDB69)]
        [DataRow("word(#fe10*#fe10)", 0xC100)]
        [DataRow("word(#1234*#1234)", 0x5A90)]
        public void WordWithIntegerWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("word(254.0)")]
        [DataRow("word(254.1*254.1)")]
        public void WordWithRealFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("word(\"\")")]
        [DataRow("word(\"abc\")")]
        public void WordWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("word(truncate(2540000.0))", 0xC1E0)]
        [DataRow("word(truncate(2543.1*2543.1))", 0xAF1D)]
        public void WordWithRealAndTruncateWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        public void RndWorksWithNoArgument()
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("");

            // --- Act
            var exprNode = ParseExpr("rnd()");

            // --- Assert
            var result = assembler.Eval(null, exprNode);
            result.IsValid.ShouldBeTrue();
            result.Type.ShouldBe(ExpressionValueType.Integer);
        }

        [TestMethod]
        [DataRow("rnd(8, 11)", 8, 11)]
        [DataRow("rnd(123, 123)", 123, 123)]
        public void RndWorksWithArgument(string source, int lower, int upper)
        {
            // --- Arrange
            var assembler = new Z80Assembler();
            assembler.Compile("");

            // --- Act
            var exprNode = ParseExpr(source);

            // --- Assert
            var result = assembler.Eval(null, exprNode);
            result.IsValid.ShouldBeTrue();
            result.Type.ShouldBe(ExpressionValueType.Integer);
            var value = result.AsLong();
            value.ShouldBeGreaterThanOrEqualTo(lower);
            value.ShouldBeLessThanOrEqualTo(upper);
        }

        [TestMethod]
        [DataRow("length(true)")]
        [DataRow("length(false)")]
        [DataRow("length(2)")]
        [DataRow("length(100)")]
        [DataRow("length(2.1)")]
        [DataRow("length(100.2)")]
        [DataRow("len(true)")]
        [DataRow("len(false)")]
        [DataRow("len(2)")]
        [DataRow("len(100)")]
        [DataRow("len(2.1)")]
        [DataRow("len(100.2)")]
        public void LengthFailsWithInvalidArgument(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("length(\"\")", 0)]
        [DataRow("length(\"hello\")", 5)]
        [DataRow("length(\"you\")", 3)]
        [DataRow("len(\"\")", 0)]
        [DataRow("len(\"hello\")", 5)]
        [DataRow("len(\"you\")", 3)]
        public void LengthWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("left(\"Hello\", 1)", "H")]
        [DataRow("left(\"Hello\", 3)", "Hel")]
        [DataRow("left(\"Hello\", 8)", "Hello")]
        public void LeftWorksAsExpected(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("right(\"Hello\", 1)", "o")]
        [DataRow("right(\"Hello\", 3)", "llo")]
        [DataRow("right(\"Hello\", 8)", "Hello")]
        public void RightWorksAsExpected(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("substr(\"Hello\", 0, 1)", "H")]
        [DataRow("substr(\"Hello\", 1, 3)", "ell")]
        [DataRow("substr(\"Hello\", 8, 8)", "")]
        public void SubWorksAsExpected(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("fill(\"Q\", 3)", "QQQ")]
        [DataRow("fill(\"He\", 4)", "HeHeHeHe")]
        public void FillWorksAsExpected(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("MySymbol .equ fill(\"QQQ\", 30000)")]
        public void FillWithTooLongResultRaisesException(string source)
        {
            // --- Arrange
            var compiler = new Z80Assembler();

            // --- Act
            var output = compiler.Compile(source);

            // --- Assert
            output.ErrorCount.ShouldBe(1);
            output.Errors[0].ErrorCode.ShouldBe(Errors.Z0200);
        }

        [TestMethod]
        [DataRow("int(true)", 1)]
        [DataRow("int(false)", 0)]
        public void IntWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("int(2)", 2)]
        [DataRow("int(3)", 3)]
        public void IntWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("int(2.0)", 2)]
        [DataRow("int(3.0)", 3)]
        [DataRow("int(3.25)", 3)]
        [DataRow("int(3.75)", 3)]
        public void IntWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("int(\"fail\")")]
        [DataRow("int(\"\")")]
        public void IntWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("frac(true)", 0.0)]
        [DataRow("frac(false)", 0.0)]
        public void FracWithBoolWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("frac(2)", 0.0)]
        [DataRow("frac(3)", 0.0)]
        public void FracWithIntWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("frac(2.0)", 0.0)]
        [DataRow("frac(3.0)", 0.0)]
        [DataRow("frac(3.25)", 0.25)]
        [DataRow("frac(3.75)", 0.75)]
        public void FracWithRealWorks(string source, double expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("frac(\"fail\")")]
        [DataRow("frac(\"\")")]
        public void FracWithStringFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("lowercase(false)")]
        [DataRow("lowercase(12)")]
        [DataRow("lowercase(12.5)")]
        [DataRow("lcase(false)")]
        [DataRow("lcase(12)")]
        [DataRow("lcase(12.5)")]
        public void LowerCaseWithInvalidArgsFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("lowercase(\"AbC\")", "abc")]
        [DataRow("lcase(\"AbC\")", "abc")]
        public void LowerCaseWithStringWorks(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("uppercase(false)")]
        [DataRow("uppercase(12)")]
        [DataRow("uppercase(12.5)")]
        [DataRow("ucase(false)")]
        [DataRow("ucase(12)")]
        [DataRow("ucase(12.5)")]
        public void UpperCaseWithInvalidArgsFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("uppercase(\"abC\")", "ABC")]
        [DataRow("ucase(\"abC\")", "ABC")]
        public void UpperCaseWithStringWorks(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("str(\"abC\")", "abC")]
        [DataRow("str(false)", "false")]
        [DataRow("str(true)", "true")]
        [DataRow("str(123+123)", "246")]
        [DataRow("str(123.1+123.0)", "246.1")]
        public void StrWorksAsExpected(string source, string expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("ink(0)", 0x00)]
        [DataRow("ink(1)", 0x01)]
        [DataRow("ink(2)", 0x02)]
        [DataRow("ink(3)", 0x03)]
        [DataRow("ink(4)", 0x04)]
        [DataRow("ink(5)", 0x05)]
        [DataRow("ink(6)", 0x06)]
        [DataRow("ink(7)", 0x07)]
        [DataRow("ink(8)", 0x00)]
        public void InkWithIntegerWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("paper(0)", 0x00)]
        [DataRow("paper(1)", 0x08)]
        [DataRow("paper(2)", 0x10)]
        [DataRow("paper(3)", 0x18)]
        [DataRow("paper(4)", 0x20)]
        [DataRow("paper(5)", 0x28)]
        [DataRow("paper(6)", 0x30)]
        [DataRow("paper(7)", 0x38)]
        [DataRow("paper(9)", 0x08)]
        public void PaperWithIntegerWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("bright(0)", 0x00)]
        [DataRow("bright(1)", 0x40)]
        [DataRow("bright(-1)", 0x40)]
        [DataRow("bright(2)", 0x40)]
        public void BrightWithIntegerWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("flash(0)", 0x00)]
        [DataRow("flash(1)", 0x80)]
        [DataRow("flash(-1)", 0x80)]
        [DataRow("flash(3)", 0x80)]
        public void FlashWithIntegerWorks(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("attr(0, 0, 0, 0)", 0x00)]
        [DataRow("attr(1, 0, 0, 0)", 0x01)]
        [DataRow("attr(5, 0, 0, 0)", 0x05)]
        [DataRow("attr(9, 0, 0, 0)", 0x01)]
        [DataRow("attr(1, 3, 0, 0)", 0x19)]
        [DataRow("attr(5, 3, 0, 0)", 0x1D)]
        [DataRow("attr(9, 3, 0, 0)", 0x19)]
        [DataRow("attr(1, 11, 0, 0)", 0x19)]
        [DataRow("attr(5, 11, 0, 0)", 0x1D)]
        [DataRow("attr(9, 11, 0, 0)", 0x19)]

        [DataRow("attr(0, 0, 1, 0)", 0x40)]
        [DataRow("attr(1, 0, 1, 0)", 0x41)]
        [DataRow("attr(5, 0, 1, 0)", 0x45)]
        [DataRow("attr(9, 0, 1, 0)", 0x41)]
        [DataRow("attr(1, 3, 1, 0)", 0x59)]
        [DataRow("attr(5, 3, 1, 0)", 0x5D)]
        [DataRow("attr(9, 3, 1, 0)", 0x59)]
        [DataRow("attr(1, 11, 1, 0)", 0x59)]
        [DataRow("attr(5, 11, 1, 0)", 0x5D)]
        [DataRow("attr(9, 11, 1, 0)", 0x59)]

        [DataRow("attr(0, 0, 1, 1)", 0xC0)]
        [DataRow("attr(1, 0, 1, 1)", 0xC1)]
        [DataRow("attr(5, 0, 1, 1)", 0xC5)]
        [DataRow("attr(9, 0, 1, 1)", 0xC1)]
        [DataRow("attr(1, 3, 1, 1)", 0xD9)]
        [DataRow("attr(5, 3, 1, 1)", 0xDD)]
        [DataRow("attr(9, 3, 1, 1)", 0xD9)]
        [DataRow("attr(1, 11, 1, 1)", 0xD9)]
        [DataRow("attr(5, 11, 1, 1)", 0xDD)]
        [DataRow("attr(9, 11, 1, 1)", 0xD9)]

        [DataRow("attr(0, 0, 0, 1)", 0x80)]
        [DataRow("attr(1, 0, 0, 1)", 0x81)]
        [DataRow("attr(5, 0, 0, 1)", 0x85)]
        [DataRow("attr(9, 0, 0, 1)", 0x81)]
        [DataRow("attr(1, 3, 0, 1)", 0x99)]
        [DataRow("attr(5, 3, 0, 1)", 0x9D)]
        [DataRow("attr(9, 3, 0, 1)", 0x99)]
        [DataRow("attr(1, 11, 0, 1)", 0x99)]
        [DataRow("attr(5, 11, 0, 1)", 0x9D)]
        [DataRow("attr(9, 11, 0, 1)", 0x99)]

        [DataRow("attr(0, 0, 0)", 0x00)]
        [DataRow("attr(1, 0, 0)", 0x01)]
        [DataRow("attr(5, 0, 0)", 0x05)]
        [DataRow("attr(9, 0, 0)", 0x01)]
        [DataRow("attr(1, 3, 0)", 0x19)]
        [DataRow("attr(5, 3, 0)", 0x1D)]
        [DataRow("attr(9, 3, 0)", 0x19)]
        [DataRow("attr(1, 11, 0)", 0x19)]
        [DataRow("attr(5, 11, 0)", 0x1D)]
        [DataRow("attr(9, 11, 0)", 0x19)]

        [DataRow("attr(0, 0)", 0x00)]
        [DataRow("attr(1, 0)", 0x01)]
        [DataRow("attr(5, 0)", 0x05)]
        [DataRow("attr(9, 0)", 0x01)]
        [DataRow("attr(1, 3)", 0x19)]
        [DataRow("attr(5, 3)", 0x1D)]
        [DataRow("attr(9, 3)", 0x19)]
        [DataRow("attr(1, 11)", 0x19)]
        [DataRow("attr(5, 11)", 0x1D)]
        [DataRow("attr(9, 11)", 0x19)]
        public void AttrWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("scraddr(0, 0)", 0x4000)]
        [DataRow("scraddr(0, 3)", 0x4000)]
        [DataRow("scraddr(0, 7)", 0x4000)]
        [DataRow("scraddr(0, 8)", 0x4001)]
        [DataRow("scraddr(1, 0)", 0x4100)]
        [DataRow("scraddr(1, 3)", 0x4100)]
        [DataRow("scraddr(1, 7)", 0x4100)]
        [DataRow("scraddr(1, 8)", 0x4101)]
        [DataRow("scraddr(3, 0)", 0x4300)]
        [DataRow("scraddr(3, 3)", 0x4300)]
        [DataRow("scraddr(3, 7)", 0x4300)]
        [DataRow("scraddr(3, 8)", 0x4301)]
        [DataRow("scraddr(7, 255)", 0x471f)]
        [DataRow("scraddr(7, 254)", 0x471f)]
        [DataRow("scraddr(7, 246)", 0x471e)]
        [DataRow("scraddr(7, 244)", 0x471e)]
        [DataRow("scraddr(8, 0)", 0x4020)]
        [DataRow("scraddr(8, 3)", 0x4020)]
        [DataRow("scraddr(8, 7)", 0x4020)]
        [DataRow("scraddr(8, 8)", 0x4021)]
        [DataRow("scraddr(9, 0)", 0x4120)]
        [DataRow("scraddr(9, 3)", 0x4120)]
        [DataRow("scraddr(9, 7)", 0x4120)]
        [DataRow("scraddr(9, 8)", 0x4121)]
        [DataRow("scraddr(64, 0)", 0x4800)]
        [DataRow("scraddr(64, 3)", 0x4800)]
        [DataRow("scraddr(64, 7)", 0x4800)]
        [DataRow("scraddr(64, 8)", 0x4801)]
        [DataRow("scraddr(65, 0)", 0x4900)]
        [DataRow("scraddr(65, 3)", 0x4900)]
        [DataRow("scraddr(65, 7)", 0x4900)]
        [DataRow("scraddr(65, 8)", 0x4901)]
        [DataRow("scraddr(128, 0)", 0x5000)]
        [DataRow("scraddr(128, 3)", 0x5000)]
        [DataRow("scraddr(128, 7)", 0x5000)]
        [DataRow("scraddr(128, 8)", 0x5001)]
        [DataRow("scraddr(129, 0)", 0x5100)]
        [DataRow("scraddr(129, 3)", 0x5100)]
        [DataRow("scraddr(129, 7)", 0x5100)]
        [DataRow("scraddr(129, 8)", 0x5101)]
        [DataRow("scraddr(191, 255)", 0x57ff)]
        [DataRow("scraddr(190, 255)", 0x56ff)]
        public void ScrAddrWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("scraddr(-1, 0)")]
        [DataRow("scraddr(192, 0)")]
        [DataRow("scraddr(0, -1)")]
        [DataRow("scraddr(0, 256)")]
        public void ScrAddrWithInvalidArgsFails(string source)
        {
            EvalFails(source);
        }

        [TestMethod]
        [DataRow("attraddr(0, 0)", 0x5800)]
        [DataRow("attraddr(0, 4)", 0x5800)]
        [DataRow("attraddr(0, 8)", 0x5801)]
        [DataRow("attraddr(7, 0)", 0x5800)]
        [DataRow("attraddr(7, 4)", 0x5800)]
        [DataRow("attraddr(7, 8)", 0x5801)]
        [DataRow("attraddr(8, 0)", 0x5820)]
        [DataRow("attraddr(8, 4)", 0x5820)]
        [DataRow("attraddr(8, 8)", 0x5821)]
        [DataRow("attraddr(70, 255)", 0x591f)]
        [DataRow("attraddr(130, 25)", 0x5a03)]
        [DataRow("attraddr(191, 255)", 0x5aff)]
        public void AttrAddrWorksAsExpected(string source, int expected)
        {
            EvalExpression(source, expected);
        }

        [TestMethod]
        [DataRow("attraddr(-1, 0)")]
        [DataRow("attraddr(192, 0)")]
        [DataRow("attraddr(0, -1)")]
        [DataRow("attraddr(0, 256)")]
        public void AttrAddrWithInvalidArgsFails(string source)
        {
            EvalFails(source);
        }

    }
}
