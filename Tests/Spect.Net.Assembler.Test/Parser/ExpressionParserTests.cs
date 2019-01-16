using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Test.Parser
{
    [TestClass]
    public class ExpressionParserTests : ParserTestBed
    {
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
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedOrOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 | 1111 | 23456");

            // --- Assert
            var binaryOp = expr as BitwiseOrOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as BitwiseOrOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void XorOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 ^ 23456");

            // --- Assert
            var literal = expr as BitwiseXorOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedXorOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 ^ 1111 ^ 23456");

            // --- Assert
            var binaryOp = expr as BitwiseXorOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as BitwiseXorOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void AndOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 & 23456");

            // --- Assert
            var literal = expr as BitwiseAndOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedAndOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 & 1111 & 23456");

            // --- Assert
            var binaryOp = expr as BitwiseAndOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as BitwiseAndOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ShiftLeftOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 << 23456");

            // --- Assert
            var literal = expr as ShiftLeftOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ShiftRightOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 >> 23456");

            // --- Assert
            var literal = expr as ShiftRightOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedShiftLeftOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 << 1111 << 23456");

            // --- Assert
            var binaryOp = expr as ShiftLeftOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as ShiftLeftOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedShiftRightOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 >> 1111 >> 23456");

            // --- Assert
            var binaryOp = expr as ShiftRightOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as ShiftRightOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedShiftOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 >> 1111 << 23456");

            // --- Assert
            var binaryOp = expr as ShiftLeftOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as ShiftRightOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void AddOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 + 23456");

            // --- Assert
            var literal = expr as AddOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void SubtractOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 - 23456");

            // --- Assert
            var literal = expr as SubtractOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedAddOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 + 1111 + 23456");

            // --- Assert
            var binaryOp = expr as AddOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as AddOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedSubtractOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 - 1111 - 23456");

            // --- Assert
            var binaryOp = expr as SubtractOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as SubtractOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedAdditiveOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 + 1111 - 23456");

            // --- Assert
            var binaryOp = expr as SubtractOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as AddOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void MultiplyOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 * 23456");

            // --- Assert
            var literal = expr as MultiplyOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void DivideOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 / 23456");

            // --- Assert
            var literal = expr as DivideOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ModuloOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 % 23456");

            // --- Assert
            var literal = expr as ModuloOperationNode;
            literal.ShouldNotBeNull();
            var left = literal.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = literal.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedMultiplicationOperatorParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("12345 * 1111 / 23456");

            // --- Assert
            var binaryOp = expr as DivideOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as MultiplyOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedMultiplicationOperatorParsingWorks2()
        {
            // --- Act
            var expr = ParseExpr("12345 / 1111 % 23456");

            // --- Assert
            var binaryOp = expr as ModuloOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as DivideOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void ChainedMultiplicationOperatorParsingWorks3()
        {
            // --- Act
            var expr = ParseExpr("12345 % 1111 * 23456");

            // --- Assert
            var binaryOp = expr as MultiplyOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as ModuloOperationNode;
            left.ShouldNotBeNull();
            var value1 = left.LeftOperand as LiteralNode;
            value1.ShouldNotBeNull();
            value1.AsWord.ShouldBe((ushort)12345);
            var value2 = left.RightOperand as LiteralNode;
            value2.ShouldNotBeNull();
            value2.AsWord.ShouldBe((ushort)1111);
            var right = binaryOp.RightOperand as LiteralNode;
            right.ShouldNotBeNull();
            right.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void UnaryPlusParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("+12345");

            // --- Assert
            var unary = expr as UnaryPlusNode;
            unary.ShouldNotBeNull();
            var value = unary.Operand as LiteralNode;
            value.ShouldNotBeNull();
            value.AsWord.ShouldBe((ushort)12345);
        }

        [TestMethod]
        public void UnaryMinusParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("-12345");

            // --- Assert
            var unary = expr as UnaryMinusNode;
            unary.ShouldNotBeNull();
            var value = unary.Operand as LiteralNode;
            value.ShouldNotBeNull();
            value.AsWord.ShouldBe((ushort)12345);
        }

        [TestMethod]
        public void UnaryBitwiseNotParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("~12345");

            // --- Assert
            var unary = expr as UnaryBitwiseNotNode;
            unary.ShouldNotBeNull();
            var value = unary.Operand as LiteralNode;
            value.ShouldNotBeNull();
            value.AsWord.ShouldBe((ushort)12345);
        }


        [TestMethod]
        public void BracesInExpressionOperatorParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("12345 % [1111 + 23456]");

            // --- Assert
            var binaryOp = expr as ModuloOperationNode;
            binaryOp.ShouldNotBeNull();
            var left = binaryOp.LeftOperand as LiteralNode;
            left.ShouldNotBeNull();
            left.AsWord.ShouldBe((ushort)12345);
            var right = binaryOp.RightOperand as AddOperationNode;
            right.ShouldNotBeNull();
            var leftNode = right.LeftOperand as LiteralNode;
            leftNode.ShouldNotBeNull();
            leftNode.AsWord.ShouldBe((ushort)1111);
            var rightNode = right.RightOperand as LiteralNode;
            rightNode.ShouldNotBeNull();
            rightNode.AsWord.ShouldBe((ushort)23456);
        }

        [TestMethod]
        public void IdentifierParsingWorks()
        {
            // --- Act
            var expr = ParseExpr("Symbol");

            // --- Assert
            var ident = expr as IdentifierNode;
            ident.ShouldNotBeNull();
            ident.SymbolName.ShouldBe("SYMBOL");
        }

        [TestMethod]
        [DataRow("$")]
        [DataRow(".")]
        [DataRow("*")]
        public void CurrentAssemblyAddressParsingWorks(string source)
        {
            // --- Act
            var expr = ParseExpr(source);

            // --- Assert
            var ident = expr as CurrentAddressNode;
            ident.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow(".cnt")]
        [DataRow(".CNT")]
        [DataRow("$cnt")]
        [DataRow("$CNT")]
        public void CurrentLoopCounterParsingWorks(string source)
        {
            // --- Act
            var expr = ParseExpr(source);

            // --- Assert
            var ident = expr as CurrentLoopCounterNode;
            ident.ShouldNotBeNull();
        }

        [TestMethod]
        public void ConditionalOperatorParsingWorks1()
        {
            // --- Act
            var expr = ParseExpr("23+11 > 3 ? 123 : 456");

            // --- Assert
            var conditional = expr as ConditionalExpressionNode;
            conditional.ShouldNotBeNull();
        }

        [TestMethod]
        public void FunctionWithNoArgumentsWorks()
        {
            // --- Act
            var expr = ParseExpr("func()");

            // --- Assert
            var func = expr as FunctionInvocationNode;
            func.ShouldNotBeNull();
            func.FunctionName.ShouldBe("func");
            func.ArgumentExpressions.Count.ShouldBe(0);
        }

        [TestMethod]
        public void FunctionWithSingleArgumentsWorks()
        {
            // --- Act
            var expr = ParseExpr("func(#123)");

            // --- Assert
            var func = expr as FunctionInvocationNode;
            func.ShouldNotBeNull();
            func.FunctionName.ShouldBe("func");
            func.ArgumentExpressions.Count.ShouldBe(1);
            var arg = func.ArgumentExpressions[0] as LiteralNode;
            arg.ShouldNotBeNull();
        }

        [TestMethod]
        public void FunctionWithMultipleArgumentsWorks()
        {
            // --- Act
            var expr = ParseExpr("func(#123, 12 * 3, 12 + 3)");

            // --- Assert
            var func = expr as FunctionInvocationNode;
            func.ShouldNotBeNull();
            func.FunctionName.ShouldBe("func");
            func.ArgumentExpressions.Count.ShouldBe(3);
            var arg1 = func.ArgumentExpressions[0] as LiteralNode;
            arg1.ShouldNotBeNull();
            var arg2 = func.ArgumentExpressions[1] as MultiplyOperationNode;
            arg2.ShouldNotBeNull();
            var arg3 = func.ArgumentExpressions[2] as AddOperationNode;
            arg3.ShouldNotBeNull();
        }
    }
}
