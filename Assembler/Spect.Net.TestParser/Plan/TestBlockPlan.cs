using System.Collections.Generic;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Respresent a test block plan
    /// </summary>
    public class TestBlockPlan
    {
        /// <summary>
        /// Parent test set
        /// </summary>
        public TestSetPlan TestSet { get; }

        /// <summary>
        /// ID of the test block
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// ID of the test category
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Disable the interrupt when running test code
        /// </summary>
        public bool? DisableInterrupt { get; set; }

        /// <summary>
        /// Test timeout in milliseconds
        /// </summary>
        public int? TimeoutValue { get; set; }

        /// <summary>
        /// Act of the test block
        /// </summary>
        public InvokePlanBase Act { get; set; }

        /// <summary>
        /// List of breakpoints
        /// </summary>
        public List<ushort> Breakpoints { get; } = new List<ushort>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestBlockPlan(TestSetPlan testSet, string id, string category)
        {
            Id = id;
            Category = category;
            TestSet = testSet;
        }
    }
}