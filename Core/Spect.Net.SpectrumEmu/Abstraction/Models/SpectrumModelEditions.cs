using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This class describes the parameters of a Spectrum model
    /// </summary>
    public class SpectrumModelEditions
    {
        /// <summary>
        /// The available revisions of this Spectrum model
        /// </summary>
        public Dictionary<string, SpectrumEdition> Editions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpectrumModelEditions()
        {
            Editions = new Dictionary<string, SpectrumEdition>();
        }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public SpectrumModelEditions Clone()
        {
            return new SpectrumModelEditions
            {
                Editions = Editions.ToDictionary(v => v.Key, v => v.Value.Clone())
            };
        }
    }
}