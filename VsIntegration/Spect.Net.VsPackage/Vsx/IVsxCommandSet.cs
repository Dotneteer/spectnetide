using System;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This interface represents a VSX command set
    /// </summary>
    public interface IVsxCommandSet : IVsxPackageSiteable
    {
        /// <summary>
        /// The command set identifier (Guid)
        /// </summary>
        Guid Guid { get; }
    }
}