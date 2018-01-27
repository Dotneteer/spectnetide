namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// This class represents the execution plan of a TestSet
    /// </summary>
    public class TestSetPlan
    {
        /// <summary>
        /// Type of the machine
        /// </summary>
        public MachineType? MachineType { get; set; }


    }

    /// <summary>
    /// Type of the machine
    /// </summary>
    public enum MachineType
    {
        Spectrum48,
        Spectrum128,
        SpectrumP3,
        Next
    }
}