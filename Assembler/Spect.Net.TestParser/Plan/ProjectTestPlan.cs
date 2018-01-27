using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// This class represents the test plan of a ZX Spectrum project
    /// </summary>
    public class ProjectTestPlan
    {
        private readonly Dictionary<string, TestSetPlan> _testSetPlans =
            new Dictionary<string, TestSetPlan>();

        /// <summary>
        /// Gets the test set plans of this project
        /// </summary>
        public IReadOnlyDictionary<string, TestSetPlan> TestSetPlans 
            => new ReadOnlyDictionary<string, TestSetPlan>(_testSetPlans);
    }
}