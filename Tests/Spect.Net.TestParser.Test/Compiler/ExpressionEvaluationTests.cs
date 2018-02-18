using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Plan;
using Spect.Net.TestParser.SyntaxTree.Expressions;
// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.TestParser.Test.Compiler
{
    [TestClass]
    public class ExpressionEvaluationTests: CompilerTestBed
    {
        [TestMethod]
        public void AddWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 + number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(12345+23456);
        }

        [TestMethod]
        public void AddWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 + text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsByteArray().ShouldBe(new byte[] {0x31, 0x32, 0x33, 0x34, 0x35, 0x32, 0x33, 0x34, 0x35, 0x36});
        }

        [TestMethod]
        public void SubtractWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 - number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(12345 - 23456);
        }

        [TestMethod]
        public void MultiplyWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 * number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(12345 * 23456);
        }

        [TestMethod]
        public void DivideWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 / number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456 / 12345);
        }

        [TestMethod]
        public void ModuloWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 % number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456 % 12345);
        }

        [TestMethod]
        public void ShiftRightWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 >> number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456 >> 12345);
        }

        [TestMethod]
        public void ShiftLeftWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 << number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456L << 12345);
        }

        [TestMethod]
        public void EqualWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 == number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe(23456 == 12345);
        }

        [TestMethod]
        public void EqualWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text2 == text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe("12345" == "23456");
        }

        [TestMethod]
        public void NotEqualWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 != number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe(23456 != 12345);
        }

        [TestMethod]
        public void NotEqualWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text2 != text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe("12345" != "23456");
        }

        [TestMethod]
        public void LessThanWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 < number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe(23456 < 12345);
        }

        [TestMethod]
        public void LessThanOrEqualWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 <= number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe(23456 <= 12345);
        }

        [TestMethod]
        public void GreaterThanWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 > number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe(23456 > 12345);
        }

        [TestMethod]
        public void GreaterThanOrEqualWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 >= number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBe(23456 >= 12345);
        }

        [TestMethod]
        public void BitwiseOrWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 | number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456 | 12345);
        }

        [TestMethod]
        public void BitwiseOrWithByteArrayWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 | text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsByteArray().ShouldBe(new byte[] { 0x31 | 0x32, 0x32 | 0x33, 0x33 | 0x34, 0x34 | 0x35, 0x35 | 0x36 });
        }

        [TestMethod]
        public void BitwiseAndWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 & number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456 & 12345);
        }

        [TestMethod]
        public void BitwiseAndWithByteArrayWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 & text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsByteArray().ShouldBe(new byte[] { 0x31 & 0x32, 0x32 & 0x33, 0x33 & 0x34, 0x34 & 0x35, 0x35 & 0x36 });
        }

        [TestMethod]
        public void BitwiseXorWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number2 ^ number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsNumber().ShouldBe(23456 ^ 12345);
        }

        [TestMethod]
        public void BitwiseXorWithByteArrayWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 ^ text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsByteArray().ShouldBe(new byte[] { 0x31 ^ 0x32, 0x32 ^ 0x33, 0x33 ^ 0x34, 0x34 ^ 0x35, 0x35 ^ 0x36 });
        }

        [TestMethod]
        public void BitwiseNotWithByteArrayWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("~text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            unchecked
            {
                result.value.AsByteArray().ShouldBe(new [] { (byte)~0x31, (byte)~0x32, (byte)~0x33, (byte)~0x34, (byte)~0x35 });
            }
        }

        [TestMethod]
        public void LogicalNotWithByteArrayWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("!text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
            result.value.AsBool().ShouldBeFalse();
        }

        private (TestFilePlan plan, ExpressionValue value) EvalExpression(string expr)
        {
            var code = @"
            testset MyTestSet
            {{
                source ""Simple.z80asm"";
                data
                {{
                    number1: 12345;
                    text1: ""12345"";
                    number2: 23456;
                    text2: ""23456"";
                }}
                
                test MyTest
                {{
                    arrange
                    {{
                        hl: {0};
                    }}
                    act call #0000;
                }}
            }}
            ";
            var program = string.Format(code, expr);
            var plan = Compile(program);
            if (plan.Errors.Count > 0) return (plan, null);
            var testBlock = plan.TestSetPlans[0].TestBlocks[0];
            var arrange = testBlock.ArrangeAssignments[0] as RunTimeRegisterAssignmentPlan;
            var value = arrange.Value.Evaluate(testBlock);
            return (plan, value);
        }


    }
}