using System;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class represents a tool window
    /// </summary>
    public class ToolWindowAttribute: Attribute
    {
        /// <summary>
        /// The type of the tool window
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Attribute" /> class.
        /// </summary>
        public ToolWindowAttribute(Type type)
        {
            Type = type;
        }
    }
}