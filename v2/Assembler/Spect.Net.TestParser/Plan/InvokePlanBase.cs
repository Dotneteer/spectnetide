namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Respresents an abstract invoke plan
    /// </summary>
    public class InvokePlanBase
    {
    }

    /// <summary>
    /// Represents a call plan
    /// </summary>
    public class CallPlan : InvokePlanBase
    {
        /// <summary>
        /// Start address
        /// </summary>
        public ushort Address { get; }

        public CallPlan(ushort address)
        {
            Address = address;
        }
    }

    /// <summary>
    /// Represents a start plan
    /// </summary>
    public class StartPlan : InvokePlanBase
    {
        /// <summary>
        /// Start address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Stop address, if null, then HALT stops
        /// </summary>
        public ushort? StopAddress { get; }

        public StartPlan(ushort address, ushort? stopAddress)
        {
            Address = address;
            StopAddress = stopAddress;
        }
    }
}