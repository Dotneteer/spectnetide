using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.SyntaxTree.TestBlock
{
    /// <summary>
    /// This class represents a test block of the language
    /// </summary>
    public class TestBlock : LanguageBlockBase
    {
        /// <summary>
        /// The 'test' keyword span
        /// </summary>
        public TextSpan TestKeywordSpan { get; set; }

        /// <summary>
        /// The ID of the test
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// The span of the test ID
        /// </summary>
        public TextSpan TestIdSpan { get; set; }

        /// <summary>
        /// The 'test' keyword span
        /// </summary>
        public TextSpan? CategoryKeywordSpan { get; set; }

        /// <summary>
        /// The category of the test
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The category ID span
        /// </summary>
        public TextSpan? CategoryIdSpan { get; set; }

        /// <summary>
        /// The 'machine' keyword span
        /// </summary>
        public TextSpan? MachineKeywordSpan { get; set; }

        /// <summary>
        /// The test machine ID
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// The machine ID span
        /// </summary>
        public TextSpan? MachineIdSpan { get; set; }

        /// <summary>
        /// The source contex clause
        /// </summary>
        public SourceContextClause SourceContext { get; set; }

        /// <summary>
        /// The test options clause
        /// </summary>
        public TestOptionsClause TestOptions { get; set; }

        /// <summary>
        /// The 'params' keyword span
        /// </summary>
        public TextSpan? ParamsKeywordSpan { get; set; }

        /// <summary>
        /// The list of test parameters
        /// </summary>
        public List<IdentifierClause> Params { get; }

        /// <summary>
        /// The 'case' keyword span
        /// </summary>
        public TextSpan? CaseKeywordSpan { get; set; }

        /// <summary>
        /// The list of test cases
        /// </summary>
        public List<ExpressionNode> Cases { get; }

        /// <summary>
        /// The 'arrange' keyword span
        /// </summary>
        public TextSpan? ArrangeKeywordSpan { get; set; }

        /// <summary>
        /// The list of arrange assignaments
        /// </summary>
        public List<AssignmentClause> ArrangeAssignments { get; }

        /// <summary>
        /// The 'act' keyword span
        /// </summary>
        public TextSpan? ActKeywordSpan { get; set; }

        /// <summary>
        /// The 'start' keyword span
        /// </summary>
        public TextSpan? StartKeywordSpan { get; set; }

        /// <summary>
        /// Start expression
        /// </summary>
        public ExpressionNode StartExpr { get; set; }
        
        /// <summary>
        /// The 'stop' keyword span
        /// </summary>
        public TextSpan? StopKeywordSpan { get; set; }

        /// <summary>
        /// Stop expression
        /// </summary>
        public ExpressionNode StopExpr { get; set; }

        /// <summary>
        /// The 'halt' keyword span
        /// </summary>
        public TextSpan? HaltKeywordSpan { get; set; }

        /// <summary>
        /// The 'assert' keyword span
        /// </summary>
        public TextSpan? AssertKeywordSpan { get; set; }

        /// <summary>
        /// The list of assert conditions
        /// </summary>
        public List<ExpressionNode> AssertConditions { get; }

        /// <summary>
        /// The 'end' keyword span
        /// </summary>
        public TextSpan EndKeywordSpan { get; set; }

        public TestBlock()
        {
            Params = new List<IdentifierClause>();
            Cases = new List<ExpressionNode>();
            ArrangeAssignments = new List<AssignmentClause>();
            AssertConditions = new List<ExpressionNode>();
        }
    }
}