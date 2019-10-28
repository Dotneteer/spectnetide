namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class represents the floppy configuration
    /// </summary>
    public class FloppyConfiguration: IFloppyConfiguration
    {
        /// <summary>
        /// Has the computer a floppy drive
        /// </summary>
        public bool FloppyPresent { get; set; }

        /// <summary>
        /// Is drive B present?
        /// </summary>
        public bool DriveBPresent { get; set; }

        /// <summary>
        /// Returns a clone of this instance
        /// </summary>
        /// <returns>A clone of this instance</returns>
        public FloppyConfiguration Clone()
        {
            return new FloppyConfiguration
            {
                FloppyPresent = FloppyPresent,
                DriveBPresent = DriveBPresent
            };
        }
    }
}