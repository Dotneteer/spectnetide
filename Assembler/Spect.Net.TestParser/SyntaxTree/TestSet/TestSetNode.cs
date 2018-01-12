using System.Collections.Generic;
using Antlr4.Runtime;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.DataBlock;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    public class TestSetNode: NodeBase
    {
        /// <summary>
        /// Creates a 'testset' clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TestSetNode(Z80TestParser.TestSetContext context) : base(context)
        {
            TestSetKeywordSpan = new TextSpan(context.TESTSET().Symbol);
            TestSetIdSpan = new TextSpan(context.IDENTIFIER().Symbol);
            TestSetId = context.IDENTIFIER().GetText();
            TestBlocks = new List<TestBlockNode>();
        }

        /// <summary>
        /// The 'testset' keyword span
        /// </summary>
        public TextSpan TestSetKeywordSpan { get; set; }

        /// <summary>
        /// The ID of the test set
        /// </summary>
        public string TestSetId { get; set; }

        /// <summary>
        /// The span of the test set ID
        /// </summary>
        public TextSpan TestSetIdSpan { get; set; }

        /// <summary>
        /// The machine context clause
        /// </summary>
        public MachineContextNode MachineContext { get; set; }

        /// <summary>
        /// The source contex clause
        /// </summary>
        public SourceContextNode SourceContext { get; set; }

        /// <summary>
        /// The test options clause
        /// </summary>
        public TestOptionsNode TestOptions { get; set; }

        /// <summary>
        /// The data block clause
        /// </summary>
        public DataBlockNode DataBlock { get; set; }

        /// <summary>
        /// The init clause
        /// </summary>
        public AssignmentsNode Init { get; set; }

        /// <summary>
        /// The setup clause
        /// </summary>
        public InvokeCodeNode Setup { get; set; }

        /// <summary>
        /// The cleanup clause
        /// </summary>
        public InvokeCodeNode Cleanup { get; set; }

        /// <summary>
        /// The test block of this test set
        /// </summary>
        public List<TestBlockNode> TestBlocks { get; }
    }
}