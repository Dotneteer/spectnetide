using System.Collections.Generic;
using Spect.Net.TestParser.Compiler;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// This class represents the test plans declared in a file
    /// </summary>
    public class TestFilePlan
    {
        /// <summary>
        /// The path of the compiled file
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The compiled test sets in the file
        /// </summary>
        public List<TestSetPlan> TestSetPlans { get; }

        /// <summary>
        /// The errors found during the compilation
        /// </summary>
        public List<TestCompilerErrorInfo> Errors { get; } = new List<TestCompilerErrorInfo>();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TestFilePlan(string filename)
        {
            Filename = filename;
            TestSetPlans = new List<TestSetPlan>();
        }
    }
}