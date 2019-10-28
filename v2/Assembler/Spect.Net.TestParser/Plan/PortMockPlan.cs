using System.Collections.Generic;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Respresents a portmock plan
    /// </summary>
    public class PortMockPlan
    {
        /// <summary>
        /// The mocked port's address
        /// </summary>
        public ushort PortAddress { get; }

        /// <summary>
        /// The port pulses
        /// </summary>
        public List<PortPulsePlan> Pulses { get; } = new List<PortPulsePlan>();

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public PortMockPlan(ushort portAddress)
        {
            PortAddress = portAddress;
        }

        /// <summary>
        /// Add a new pulse to the port mock
        /// </summary>
        /// <param name="pulse"></param>
        public void AddPulse(PortPulsePlan pulse)
        {
            Pulses.Add(pulse);
        }
    }
}