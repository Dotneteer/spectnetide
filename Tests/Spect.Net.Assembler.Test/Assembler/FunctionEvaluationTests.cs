using Microsoft.VisualStudio.TestTools.UnitTesting;

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


    }
}
