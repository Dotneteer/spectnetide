using System;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This attribute represents the Command ID of a
    /// particular command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandIdAttribute: Attribute
    {
        /// <summary>
        /// Command ID value
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CommandIdAttribute(int value)
        {
            Value = value;
        }
    }
}