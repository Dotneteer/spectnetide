using System;
using System.Collections.Generic;
using Shouldly;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Test
{
    public class ExpressionTestBed : AssemblerTestBed
    {
        protected void EvalExpression(string expr, ushort? expected, bool hasEvaluationError = false,
            Dictionary<string, ushort> symbols = null)
        {
            var assembler = new Z80Assembler();
            assembler.Compile("");
            if (symbols != null)
            {
                foreach (var pair in symbols)
                {
                    assembler.SetSymbolValue(pair.Key, new ExpressionValue(pair.Value));
                }
            }
            var exprNode = ParseExpr(expr);
            var result = assembler.Eval(exprNode);
            if (expected == null)
            {
                result.ShouldBeNull();
            }
            else
            {
                result.Value.ShouldBe(expected.Value);
            }
            if (hasEvaluationError)
            {
                exprNode.EvaluationError.ShouldNotBeNull();
            }
        }

        protected void EvalExpression(string expr, double? expected, bool hasEvaluationError = false,
            Dictionary<string, ushort> symbols = null)
        {
            var assembler = new Z80Assembler();
            assembler.Compile("");
            if (symbols != null)
            {
                foreach (var pair in symbols)
                {
                    assembler.SetSymbolValue(pair.Key, new ExpressionValue(pair.Value));
                }
            }
            var exprNode = ParseExpr(expr);
            var result = assembler.Eval(exprNode);
            if (expected == null)
            {
                result.ShouldBeNull();
            }
            else
            {
                Math.Abs(Math.Round(result.AsReal(), 8) - Math.Round(expected.Value, 8)).ShouldBeLessThanOrEqualTo(10*double.Epsilon);
            }
            if (hasEvaluationError)
            {
                exprNode.EvaluationError.ShouldNotBeNull();
            }
        }

        protected void EvalExpression(string expr, string expected, bool hasEvaluationError = false,
            Dictionary<string, ushort> symbols = null)
        {
            var assembler = new Z80Assembler();
            assembler.Compile("");
            if (symbols != null)
            {
                foreach (var pair in symbols)
                {
                    assembler.SetSymbolValue(pair.Key, new ExpressionValue(pair.Value));
                }
            }
            var exprNode = ParseExpr(expr);
            var result = assembler.Eval(exprNode);
            result.AsString().ShouldBe(expected);
            if (hasEvaluationError)
            {
                exprNode.EvaluationError.ShouldNotBeNull();
            }
        }

        protected void EvalFails(string expr, bool hasEvaluationError = false,
            Dictionary<string, ushort> symbols = null)
        {
            var assembler = new Z80Assembler();
            assembler.Compile("");
            if (symbols != null)
            {
                foreach (var pair in symbols)
                {
                    assembler.SetSymbolValue(pair.Key, new ExpressionValue(pair.Value));
                }
            }
            var exprNode = ParseExpr(expr);
            var result = assembler.Eval(exprNode);
            result.ShouldBeNull();
            exprNode.EvaluationError.ShouldNotBeNull();
        }
    }
}