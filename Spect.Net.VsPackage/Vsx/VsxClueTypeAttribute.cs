using System;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This attribute gives a clue to the package about the assemblies to discover.
    /// </summary>
    /// <remarks>
    /// If no such types decorate the VsxPackage type, the executing assembly
    /// is used by default.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VsxClueTypeAttribute: Attribute
    {
        /// <summary>
        /// The type that is used as a clue
        /// </summary>
        public Type Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Attribute" /> class.
        /// </summary>
        public VsxClueTypeAttribute(Type value)
        {
            Value = value;
        }
    }
}