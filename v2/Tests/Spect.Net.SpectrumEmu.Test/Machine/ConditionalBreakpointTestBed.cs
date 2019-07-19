using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.EvalParser;
using Spect.Net.EvalParser.Generated;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using BreakpointHitType = Spect.Net.SpectrumEmu.Machine.BreakpointHitType;

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class ConditionalBreakpointTestBed
    {
        protected static IBreakpointInfo CreateBreakpoint(string hitCondition, string filterCondition)
        {
            var breakpoint = new BreakpointInfo
            {
                IsCpuBreakpoint = true,
                HitType = BreakpointHitType.None
            };
            if (hitCondition != null)
            {
                var condStart = 1;
                if (hitCondition.StartsWith("<="))
                {
                    breakpoint.HitType = BreakpointHitType.LessOrEqual;
                    condStart = 2;
                }
                else if (hitCondition.StartsWith(">="))
                {
                    breakpoint.HitType = BreakpointHitType.GreaterOrEqual;
                    condStart = 2;
                }
                else if (hitCondition.StartsWith("<"))
                {
                    breakpoint.HitType = BreakpointHitType.Less;
                }
                else if (hitCondition.StartsWith(">"))
                {
                    breakpoint.HitType = BreakpointHitType.Greater;
                }
                else if (hitCondition.StartsWith("="))
                {
                    breakpoint.HitType = BreakpointHitType.Equal;
                }
                else if (hitCondition.StartsWith("*"))
                {
                    breakpoint.HitType = BreakpointHitType.Multiple;
                }
                breakpoint.HitConditionValue = ushort.Parse(hitCondition.Substring(condStart));
            }

            if (filterCondition != null)
            {
                breakpoint.FilterCondition = filterCondition;
                var inputStream = new AntlrInputStream(filterCondition);
                var lexer = new Z80EvalLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var evalParser = new Z80EvalParser(tokenStream);
                var context = evalParser.compileUnit();
                var visitor = new Z80EvalVisitor();
                var z80Expr = (Z80ExpressionNode)visitor.Visit(context);
                breakpoint.FilterExpression = z80Expr.Expression;
            }
            return breakpoint;
        }

        /// <summary>
        /// This class holds breakpoint information used to debug Z80 Assembler
        /// source code
        /// </summary>
        protected class BreakpointInfo : IBreakpointInfo
        {
            /// <summary>
            /// This flag shows that the breakpoint is assigned to source code.
            /// </summary>
            public bool IsCpuBreakpoint { get; set; }

            /// <summary>
            /// Type of breakpoint hit condition
            /// </summary>
            public BreakpointHitType HitType { get; set; }

            /// <summary>
            /// Value of the hit condition
            /// </summary>
            public ushort HitConditionValue { get; set; }

            /// <summary>
            /// Value of the filter condition
            /// </summary>
            public string FilterCondition { get; set; }

            /// <summary>
            /// The expression that represents the filter condition
            /// </summary>
            public ExpressionNode FilterExpression { get; set; }

            /// <summary>
            /// The current hit count value
            /// </summary>
            public int CurrentHitCount { get; set; }
        }
    }
}