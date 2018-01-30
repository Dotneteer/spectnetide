using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// This class represent the test plans for an entire project
    /// </summary>
    public class TestProjectPlan
    {
        private readonly List<TestFilePlan> _testFilePlans = new List<TestFilePlan>();
        
        /// <summary>
        /// The read only collection of test file plans
        /// </summary>
        public IReadOnlyList<TestFilePlan> TestFilePlans 
            => new ReadOnlyCollection<TestFilePlan>(_testFilePlans);

        /// <summary>
        /// The number of errors
        /// </summary>
        public int ErrorCount { get; private set; }

        public void Add(TestFilePlan plan)
        {
            _testFilePlans.Add(plan);
            ErrorCount += plan.Errors.Count;
        }

        /// <summary>
        /// Clears all test file plans
        /// </summary>
        public void Clear()
        {
            _testFilePlans.Clear();
            ErrorCount = 0;
        }
    }
}