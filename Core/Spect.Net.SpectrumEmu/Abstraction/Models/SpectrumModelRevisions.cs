using System.Collections.Generic;
using System.Linq;

namespace Spect.Net.SpectrumEmu.Abstraction.Models
{
    /// <summary>
    /// This class describes the parameters of a Spectrum model
    /// </summary>
    public class SpectrumModelRevisions
    {
        /// <summary>
        /// The available revisions of this Spectrum model
        /// </summary>
        public Dictionary<string, SpectrumRevision> Revisions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpectrumModelRevisions()
        {
            Revisions = new Dictionary<string, SpectrumRevision>();
        }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public SpectrumModelRevisions Clone()
        {
            return new SpectrumModelRevisions
            {
                Revisions = Revisions.ToDictionary(v => v.Key, v => v.Value.Clone())
            };
        }
    }
}