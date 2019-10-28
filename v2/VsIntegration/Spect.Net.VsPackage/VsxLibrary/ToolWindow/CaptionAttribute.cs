using System;

namespace Spect.Net.VsPackage.VsxLibrary.ToolWindow
{
    /// <summary>
    /// This attribute allows to assign a caption to a tool window
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CaptionAttribute : Attribute
    {
        /// <summary>
        /// Caption string
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Attribute" /> class.
        /// </summary>
        public CaptionAttribute(string value)
        {
            Value = value;
        }
    }
}