using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// This object provides a collection of breakpoints
    /// </summary>
    /// <remarks>
    /// In the future, this class may be updated because of
    /// performance reasons
    /// </remarks>
    public class BreakpointCollection : Dictionary<ushort, IBreakpointInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.</summary>
        public BreakpointCollection()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" /> and uses the default equality comparer for the key type.</summary>
        /// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.Dictionary`2" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="dictionary" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.</exception>
        public BreakpointCollection(IDictionary<ushort, IBreakpointInfo> dictionary) : base(dictionary)
        {
        }
    }
}