namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Represents a concrete port pulse in a test set plan
    /// </summary>
    public class PortPulsePlan
    {
        /// <summary>
        /// Port pulse value
        /// </summary>
        public byte Value { get; }

        /// <summary>
        /// Start tact of the pulse (inclusive)
        /// </summary>
        public long StartTact { get; }

        /// <summary>
        /// End tact of the pulse (inclusive)
        /// </summary>
        public long EndTact { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public PortPulsePlan(byte value, long startTact, long endTact)
        {
            Value = value;
            StartTact = startTact;
            EndTact = endTact;
        }
    }
}