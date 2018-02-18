using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.TestParser.Test.Compiler
{
    [TestClass]
    public class ExpressionCompileTests : CompilerTestBed
    {
        [TestMethod]
        public void AddWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 + number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void AddWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 + text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void AddNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 + text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void AddNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 + number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void SubtractWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 - number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void SubtractWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 - text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void SubtractNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 - text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void SubtractNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 - number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void MultiplyWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 * number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void MultiplyWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 * text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void MultiplyNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 * text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void MultiplyNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 * number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void DivideWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 / number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void DivideWithZeroFails()
        {
            // --- Act
            var result = EvalExpression("number1 / 0");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }


        [TestMethod]
        public void DivideWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 / text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void DivideNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 / text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void DivideNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 / number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ModuloWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 % number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void ModuloWithZeroFails()
        {
            // --- Act
            var result = EvalExpression("number1 % 0");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }


        [TestMethod]
        public void ModuloWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 % text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ModuloNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 % text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ModuloNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 % number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ShiftLeftWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 << number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void ShiftLeftWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 << text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ShiftLeftNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 << text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ShiftLeftNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 << number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ShiftRightWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 >> number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void ShiftRightWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 >> text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ShiftRightNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 >> text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void ShiftRightNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 >> number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void EqualsWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 == number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void EqualsWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 == text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void EqualsNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 == text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void EqualsNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 == number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void NotEqualsWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 != number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void NotEqualsWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 != text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void NotEqualsNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 != text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void NotEqualsNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 != number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void GreaterThanWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 > number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void GreaterThanWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 > text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void GreaterThanNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 > text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void GreaterThanNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 > number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void GreaterThanOrEqualWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 >= number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void GreaterThanOrEqualWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 >= text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void GreaterThanOrEqualNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 >= text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void GreaterThanOrEqualNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 >= number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void LessThanOrEqualWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 <= number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void LessThanOrEqualWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 <= text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void LessThanOrEqualNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 <= text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void LessThanOrEqualNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 <= number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void LessThanWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 < number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void LessThanWithByteArraysFails()
        {
            // --- Act
            var result = EvalExpression("text1 < text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void LessThanNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 < text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void LessThanNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 < number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void BitwiseOrWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 | number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BitwiseOrWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 | text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BitwiseOrNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 | text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void BitwiseOrNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 | number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void BitwiseAndWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 & number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BitwiseAndWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 & text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BitwiseAndNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 & text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void BitwiseAndNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 & number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void BitwiseXorWithNumbersWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("number1 ^ number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BitwiseXorWithByteArraysWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("text1 ^ text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void BitwiseXorNumberAndByteArrayFails1()
        {
            // --- Act
            var result = EvalExpression("number1 ^ text2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void BitwiseXorNumberAndByteArrayFails2()
        {
            // --- Act
            var result = EvalExpression("text1 ^ number2");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void UnaryPlusWithNumberWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("+number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void UnaryPlusWithByteArrayFails()
        {
            // --- Act
            var result = EvalExpression("+text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void UnaryMinusWithNumberWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("-number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void UnaryMinusWithByteArrayFails()
        {
            // --- Act
            var result = EvalExpression("-text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(1);
            result.plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void UnaryBitwiseNotWithNumberWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("~number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void UnaryBitwiseNotWithByteArrayWorks()
        {
            // --- Act
            var result = EvalExpression("~text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void UnaryLogicalNotWithNumberWorkAsExpected()
        {
            // --- Act
            var result = EvalExpression("!number1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        [TestMethod]
        public void UnaryLogicalNotWithByteArrayWorks()
        {
            // --- Act
            var result = EvalExpression("!text1");

            // --- Assert
            result.plan.Errors.Count.ShouldBe(0);
        }

        private (TestFilePlan plan, ushort value) EvalExpression(string expr)
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
                    act call {0};
                }}
            }}
            ";
            var program = string.Format(code, expr);
            var plan = Compile(program);
            if (plan.Errors.Count > 0) return (plan, 0);
            var act = plan.TestSetPlans[0].TestBlocks[0].Act as CallPlan;
            return (plan, act.Address);
        }
    }
}
